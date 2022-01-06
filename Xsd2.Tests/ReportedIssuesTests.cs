using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetaTest;

using Xsd2.Capitalizers;

namespace Xsd2.Tests
{
    [TestFixture(Active=false)]
    public class ReportedIssuesTests
    {
        [Test]
        public void UppercaseNullablePropertiesAreGenerated()
        {
            var options = new XsdCodeGeneratorOptions
            {
                PropertyNameCapitalizer = new FirstCharacterCapitalizer(),
                OutputNamespace = "XSD2",
                UseLists = true,
                UseNullableTypes = true,
                ExcludeImportedTypes = true,
                AttributesToRemove =
                {
                    "System.Diagnostics.DebuggerStepThroughAttribute"
                }
            };
            
            using (var o = File.CreateText(@"Schemas\Issue12.cs"))
            {
                var generator = new XsdCodeGenerator()
                {
                    Options = options,
                    OnValidateGeneratedCode = (ns, schema) =>
                    {
                        var upperCaseType = ns.Types.Cast<CodeTypeDeclaration>().Single(a => a.Name == "UpperCaseType");
                        var valueProp = (CodeMemberProperty)upperCaseType.Members.Cast<CodeTypeMember>().Single(a => a.Name == "Value");
                        Assert.IsTrue(valueProp.Type.BaseType.Contains("Nullable"));
                    }
                };
                generator.Generate(new[] { @"Schemas\Issue12.xsd" }, o);
            }
        }

        [Test]
        public void NullablePropertiesAreGeneratedWithoutUnderscore()
        {
            var options = new XsdCodeGeneratorOptions
            {
                PropertyNameCapitalizer = new FirstCharacterCapitalizer(),
                OutputNamespace = "XSD2.Issue16",
                UseLists = true,
                UseNullableTypes = true,
                ExcludeImportedTypes = true,
                AttributesToRemove =
                {
                    "System.Diagnostics.DebuggerStepThroughAttribute"
                }
            };

            using (var o = File.CreateText(@"Schemas\Issue16.cs"))
            {
                var generator = new XsdCodeGenerator()
                {
                    Options = options,
                    OnValidateGeneratedCode = (ns, schema) =>
                    {
                        var typeWithCustomElement = ns.Types.Cast<CodeTypeDeclaration>().Single(a => a.Name == "TypeWithCustomElement");
                        var valueProp = (CodeMemberProperty)typeWithCustomElement.Members.Cast<CodeTypeMember>().Single(a => a.Name == "_SomeDate");

                        var elementName = valueProp.CustomAttributes[0].Arguments.Cast<CodeAttributeArgument>().SingleOrDefault(a => a.Name == "ElementName")?.Value as CodePrimitiveExpression;
                        Assert.IsNotNull(elementName);
                        Assert.DoesNotContain(elementName.Value.ToString(), "_");
                    }
                };
                generator.Generate(new[] { @"Schemas\Issue16.xsd" }, o);
            }
        }
    }
}
