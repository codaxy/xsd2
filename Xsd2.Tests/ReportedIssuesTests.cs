using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetaTest;

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
                CapitalizeProperties = true,
                OutputNamespace = "XSD2",
                UseLists = true,
                UseNullableTypes = true,
                StripDebuggerStepThroughAttribute = true,
                ExcludeImportedTypes = true
            };

            using (var f = File.OpenRead(@"Schemas\Issue12.xsd"))
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
                generator.Generate(f, o);
            }
        }
    }
}
