using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.IO;
using System.Diagnostics;

namespace Xsd2
{
    public class XsdCodeGenerator
    {
        public XsdCodeGeneratorOptions Options { get; set; }
        public Action<CodeNamespace, XmlSchema> OnValidateGeneratedCode { get; set; }

        XmlSchemas xsds = new XmlSchemas();
        HashSet<XmlSchema> importedSchemas = new HashSet<XmlSchema>();

        public void Generate(Stream xsdInput, TextWriter output)
        {
            if (Options == null)
                Options = new XsdCodeGeneratorOptions
                {
                    UseLists = true,
                    CapitalizeProperties = true,
                    StripDebuggerStepThroughAttribute = true,
                    OutputNamespace = "Xsd2",
                    UseNullableTypes = true
                };

            if (Options.Imports != null)
            {
                foreach (var import in Options.Imports)
                {
                    if (File.Exists(import))
                    {
                        ImportImportedSchema(import);
                    }
                    else if (Directory.Exists(import))
                    {
                        foreach (var file in Directory.GetFiles("*.xsd"))
                            ImportImportedSchema(file);
                    }
                    else
                    {
                        throw new InvalidOperationException(String.Format("Import '{0}' is not a file nor a directory.", import));
                    }
                }
            }

            XmlSchema xsd = XmlSchema.Read(xsdInput, null);            
            xsds.Add(xsd);
            
            xsds.Compile(null, true);
            
            XmlSchemaImporter schemaImporter = new XmlSchemaImporter(xsds);
          

            // create the codedom
            CodeNamespace codeNamespace = new CodeNamespace(Options.OutputNamespace);
            XmlCodeExporter codeExporter = new XmlCodeExporter(codeNamespace);

            List<XmlTypeMapping> maps = new List<XmlTypeMapping>();
            foreach (XmlSchemaElement schemaElement in xsd.Elements.Values)
            {
                if (!ElementBelongsToImportedSchema(schemaElement))
                    maps.Add(schemaImporter.ImportTypeMapping(schemaElement.QualifiedName));
            }

            foreach (XmlSchemaComplexType schemaElement in xsd.Items.OfType<XmlSchemaComplexType>())
            {
                maps.Add(schemaImporter.ImportSchemaType(schemaElement.QualifiedName));
            }

            foreach (XmlSchemaSimpleType schemaElement in xsd.Items.OfType<XmlSchemaSimpleType>())
            {
                maps.Add(schemaImporter.ImportSchemaType(schemaElement.QualifiedName));
            }

            foreach (XmlTypeMapping map in maps)
            {
                codeExporter.ExportTypeMapping(map);
            }

            ImproveCodeDom(codeNamespace, xsd);

            if (OnValidateGeneratedCode != null)
                OnValidateGeneratedCode(codeNamespace, xsd);

            // Check for invalid characters in identifiers
            CodeGenerator.ValidateIdentifiers(codeNamespace);

            // output the C# code
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();

            codeProvider.GenerateCodeFromNamespace(codeNamespace, output, new CodeGeneratorOptions());
        }

        private void ImportImportedSchema(string schemaFilePath)
        {
            using (var s = File.OpenRead(schemaFilePath))
            {
                var importedSchema = XmlSchema.Read(s, null);
                xsds.Add(importedSchema);
                importedSchemas.Add(importedSchema);
            }
        }

        private bool ElementBelongsToImportedSchema(XmlSchemaElement element)
        {
            var node = element.Parent;
            while (node != null)
            {
                if (node is XmlSchema)
                {
                    var schema = (XmlSchema)node;
                    return importedSchemas.Contains(schema);
                }
                else
                    node = node.Parent;
            }
            return false;
        }

        /// <summary>
        /// Shamelessly taken from Xsd2Code project
        /// </summary>       
        private bool ContainsTypeName(XmlSchema schema, CodeTypeDeclaration type)
        {
            foreach (var item in schema.Items)
            {
                var complexItem = item as XmlSchemaComplexType;
                if (complexItem != null)
                {
                    if (complexItem.Name == type.Name)
                    {
                        return true;
                    }
                }

                var simpleItem = item as XmlSchemaSimpleType;
                if (simpleItem != null)
                {
                    if (simpleItem.Name == type.Name)
                    {
                        return true;
                    }
                }


                var elementItem = item as XmlSchemaElement;
                if (elementItem != null)
                {
                    if (elementItem.Name == type.Name)
                    {
                        return true;
                    }
                }
            }

            ////TODO: Does not work for combined anonymous types 
            ////fallback: Check if the namespace attribute of the type equals the namespace of the file.
            ////first, find the XmlType attribute.
            //foreach (CodeAttributeDeclaration attribute in type.CustomAttributes)
            //{
            //    if (attribute.Name == "System.Xml.Serialization.XmlTypeAttribute")
            //    {
            //        foreach (CodeAttributeArgument argument in attribute.Arguments)
            //        {
            //            if (argument.Name == "Namespace")
            //            {
            //                if (((CodePrimitiveExpression)argument.Value).Value == schema.TargetNamespace)
            //                {
            //                    return true;
            //                }
            //            }
            //        }
            //    }
            //}

            return false;
        }

        private void ImproveCodeDom(CodeNamespace codeNamespace, XmlSchema schema)
        {
            codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));

            if (Options.UsingNamespaces != null)
                foreach (var ns in Options.UsingNamespaces)
                    codeNamespace.Imports.Add(new CodeNamespaceImport(ns));

            var removedTypes = new List<CodeTypeDeclaration>();

            foreach (CodeTypeDeclaration codeType in codeNamespace.Types)
            {
                if (Options.ExcludeImportedTypes && Options.Imports != null && Options.Imports.Count > 0)
                    if (!ContainsTypeName(schema, codeType))
                    {
                        removedTypes.Add(codeType);
                        continue;
                    }

                if (Options.StripDebuggerStepThroughAttribute)
                {
                    foreach (CodeAttributeDeclaration att in codeType.CustomAttributes)
                    {
                        if (att.Name == "System.Diagnostics.DebuggerStepThroughAttribute")
                        {
                            codeType.CustomAttributes.Remove(att);
                            break;
                        }
                    }
                }

                var members = new Dictionary<string, CodeTypeMember>();
                foreach (CodeTypeMember member in codeType.Members)
                    members[member.Name] = member;

                bool mixedContentDetected = Options.MixedContent && members.ContainsKey("textField") && members.ContainsKey("itemsField");

                foreach (CodeTypeMember member in members.Values)
                {
                    if (member is CodeMemberField)
                    {
                        CodeMemberField field = (CodeMemberField)member;

                        if (mixedContentDetected)
                        {
                            switch (field.Name)
                            {
                                case "textField":
                                    codeType.Members.Remove(member);
                                    continue;
                                case "itemsField":
                                    field.Type = new CodeTypeReference("System.Object[]");
                                    break;
                            }
                        }

                        if (Options.UseLists && field.Type.ArrayRank > 0)
                        {
                            CodeTypeReference type = new CodeTypeReference();
                            type.BaseType = "List<" + field.Type.BaseType + ">";
                            field.Type = type;
                        }

                        if (codeType.IsEnum && Options.CapitalizeEnumValues)
                        {
                            if (Char.IsLower(member.Name[0]))
                            {
                                member.CustomAttributes.Add(new CodeAttributeDeclaration("System.Xml.Serialization.XmlEnumAttribute", new CodeAttributeArgument { Name = "", Value = new CodePrimitiveExpression(member.Name) }));
                                member.Name = Char.ToUpper(member.Name[0]) + member.Name.Substring(1);
                            }
                        }
                    }

                    if (member is CodeMemberProperty)
                    {
                        CodeMemberProperty property = (CodeMemberProperty)member;

                        if (mixedContentDetected)
                        {
                            switch (property.Name)
                            {
                                case "Text":
                                    codeType.Members.Remove(member);
                                    continue;
                                case "Items":
                                    property.Type = new CodeTypeReference("System.Object[]");
                                    property.CustomAttributes.Add(new CodeAttributeDeclaration("System.Xml.Serialization.XmlTextAttribute", new CodeAttributeArgument { Name = "", Value = new CodeTypeOfExpression(new CodeTypeReference("System.String")) }));
                                    break;
                            }
                        }

                        if (Options.UseLists && property.Type.ArrayRank > 0)
                        {
                            CodeTypeReference type = new CodeTypeReference();
                            type.BaseType = "List<" + property.Type.BaseType + ">";
                            property.Type = type;
                        }

                        if (Options.UseNullableTypes)
                        {
                            var fieldName = GetFieldName(property.Name, "Field");
                            CodeTypeMember specified;
                            if (members.TryGetValue(property.Name + "Specified", out specified))
                            {
                                var nullableProperty = new CodeMemberProperty
                                {
                                    Name = property.Name,
                                    Type = new CodeTypeReference { BaseType = "Nullable<" + property.Type.BaseType + ">" },
                                    HasGet = true,
                                    HasSet = true,
                                    Attributes = MemberAttributes.Public | MemberAttributes.Final
                                };

                                nullableProperty.GetStatements.Add(
                                    new CodeConditionStatement(new CodeVariableReferenceExpression(fieldName + "Specified"),
                                    new[] { new CodeMethodReturnStatement(new CodeVariableReferenceExpression(fieldName)) },
                                    new[] { new CodeMethodReturnStatement(new CodePrimitiveExpression()) }
                                    ));

                                nullableProperty.SetStatements.Add(
                                    new CodeConditionStatement(new CodeSnippetExpression("value != null"),
                                    new[] { new CodeAssignStatement(new CodeVariableReferenceExpression(fieldName+"Specified"), new CodePrimitiveExpression(true)),
                                        new CodeAssignStatement(new CodeVariableReferenceExpression(fieldName), new CodeSnippetExpression("value.Value")),
                                    },
                                    new[] { new CodeAssignStatement(new CodeVariableReferenceExpression(fieldName + "Specified"), new CodePrimitiveExpression(false)) }
                                ));

                                nullableProperty.CustomAttributes.Add(new CodeAttributeDeclaration { Name = "System.Xml.Serialization.XmlIgnoreAttribute" });

                                codeType.Members.Add(nullableProperty);

                                foreach (CodeAttributeDeclaration attribute in property.CustomAttributes)
                                    if (attribute.Name == "System.Xml.Serialization.XmlAttributeAttribute")
                                        attribute.Arguments.Add(new CodeAttributeArgument { Name = "AttributeName", Value = new CodePrimitiveExpression(property.Name) });

                                property.Name = "_" + property.Name;
                                specified.Name = "_" + specified.Name;
                                property = nullableProperty;
                            }
                        }

                        if (Options.CapitalizeProperties)
                        {
                            if (Char.IsLower(property.Name[0]))
                            {
                                bool attributed = false;
                                foreach (CodeAttributeDeclaration attribute in property.CustomAttributes)
                                {
                                    switch (attribute.Name)
                                    {
                                        case "System.Xml.Serialization.XmlAttributeAttribute":
                                            attributed = true;
                                            attribute.Arguments.Add(new CodeAttributeArgument { Name = "", Value = new CodePrimitiveExpression(property.Name) });
                                            break;

                                        case "System.Xml.Serialization.XmlIgnoreAttribute":
                                        case "System.Xml.Serialization.XmlElementAttribute":
                                        case "System.Xml.Serialization.XmlArrayItemAttribute":
                                            attributed = true;
                                            break;
                                    }
                                }

                                if (!attributed)
                                    property.CustomAttributes.Add(new CodeAttributeDeclaration("System.Xml.Serialization.XmlElementAttribute", new CodeAttributeArgument { Name = "", Value = new CodePrimitiveExpression(property.Name) }));

                                property.Name = property.Name.Substring(0, 1).ToUpper() + property.Name.Substring(1);
                            }
                        }
                    }
                }
            }

            foreach (var rt in removedTypes)
                codeNamespace.Types.Remove(rt);
        }

        private static string GetFieldName(string p, string suffix = null)
        {
            return p.Substring(0, 1).ToLower() + p.Substring(1) + suffix;
        }
    }

    public class XsdCodeGeneratorOptions
    {
        public bool UseLists { get; set; }
        public bool CapitalizeProperties { get; set; }
        public bool CapitalizeEnumValues { get; set; }
        public bool StripDebuggerStepThroughAttribute { get; set; }
        public bool UseNullableTypes { get; set; }
        public List<String> Imports { get; set; }
        public List<String> UsingNamespaces { get; set; }
        
        public string OutputNamespace { get; set; }

        public bool ExcludeImportedTypes { get; set; }

        public bool MixedContent { get; set; }
    }
}
