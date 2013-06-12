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
            foreach (XmlTypeMapping map in maps)
            {
                codeExporter.ExportTypeMapping(map);
            }

            ImproveCodeDom(codeNamespace);

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

        private void ImproveCodeDom(CodeNamespace codeNamespace)
        {
            codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));

            foreach (CodeTypeDeclaration codeType in codeNamespace.Types)
            {
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

                foreach (CodeTypeMember member in members.Values)
                {
                    if (member is CodeMemberField)
                    {
                        CodeMemberField field = (CodeMemberField)member;
                        if (Options.UseLists && field.Type.ArrayRank > 0)
                        {
                            CodeTypeReference type = new CodeTypeReference();
                            type.BaseType = "List<" + field.Type.BaseType + ">";
                            field.Type = type;
                        }
                    }
                    if (member is CodeMemberProperty)
                    {
                        CodeMemberProperty property = (CodeMemberProperty)member;
                        if (Options.UseLists && property.Type.ArrayRank > 0)
                        {
                            CodeTypeReference type = new CodeTypeReference();
                            type.BaseType = "List<" + property.Type.BaseType + ">";
                            property.Type = type;
                        }   

                        if (Options.UseNullableTypes)
                        {
                            var fieldName = GetFieldName(property.Name);
                            CodeTypeMember specified;
                            if (members.TryGetValue(fieldName + "Specified", out specified))
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
                                    new CodeConditionStatement(new CodeVariableReferenceExpression(fieldName + "FieldSpecified"),
                                    new[] { new CodeMethodReturnStatement(new CodeVariableReferenceExpression(fieldName + "Field")) },
                                    new[] { new CodeMethodReturnStatement(new CodePrimitiveExpression()) }
                                    ));

                                nullableProperty.SetStatements.Add(
                                    new CodeConditionStatement(new CodeSnippetExpression("value != null"),
                                    new[] { new CodeAssignStatement(new CodeVariableReferenceExpression(fieldName+"FieldSpecified"), new CodePrimitiveExpression(true)),
                                        new CodeAssignStatement(new CodeVariableReferenceExpression(fieldName+"Field"), new CodeSnippetExpression("value.Value")),
                                    },
                                    new[] { new CodeAssignStatement(new CodeVariableReferenceExpression(fieldName + "FieldSpecified"), new CodePrimitiveExpression(false)) }
                                ));

                                nullableProperty.CustomAttributes.Add(new CodeAttributeDeclaration { Name = "System.Xml.Serialization.XmlIgnoreAttribute" });

                                codeType.Members.Add(nullableProperty);

                                foreach (CodeAttributeDeclaration attribute in property.CustomAttributes)
                                    if (attribute.Name == "System.Xml.Serialization.XmlAttributeAttribute")
                                        attribute.Arguments.Add(new CodeAttributeArgument { Name = "", Value = new CodePrimitiveExpression(property.Name) });

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
                                    if (attribute.Name == "System.Xml.Serialization.XmlAttributeAttribute")
                                    {
                                        attributed = true;
                                        attribute.Arguments.Add(new CodeAttributeArgument { Name = "", Value = new CodePrimitiveExpression(property.Name) });
                                        break;
                                    }

                                    if (attribute.Name == "System.Xml.Serialization.XmlIgnoreAttribute")
                                    {
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
        public bool StripDebuggerStepThroughAttribute { get; set; }
        public bool UseNullableTypes { get; set; }
        public List<String> Imports { get; set; }
        
        public string OutputNamespace { get; set; }
    }
}
