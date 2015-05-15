using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetaTest;

namespace Xsd2.Tests
{
    [TestFixture]
    public class GenerationTests
    {
        [Test]
        public void Test1()
        {
            var options = new XsdCodeGeneratorOptions
            {
                Imports = new List<string>() { @"Schemas\MetaConfig.xsd" },
                CapitalizeProperties = true,
                OutputNamespace = "XSD2",
                UseLists = true,
                UseNullableTypes = true,
                StripDebuggerStepThroughAttribute = true,
                ExcludeImportedTypes = true
            };
            
            using (var o = File.CreateText(@"Schemas\Xsd2Config.cs"))
            {
                var generator = new XsdCodeGenerator() { Options = options };
                generator.Generate(new[] { @"Schemas\Xsd2Config.xsd" }, o);
            }
        }

        [Test(Active=false)]
        public void Test2()
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

            using (var o = File.CreateText(@"Schemas\Data.cs"))
            {
                var generator = new XsdCodeGenerator() { Options = options };
                generator.Generate(new[] { @"Schemas\Data.xsd" }, o);
            }
        }

        [Test(Active = true)]
        public void Test3()
        {
            var options = new XsdCodeGeneratorOptions
            {
                CapitalizeProperties = true,
                OutputNamespace = "XSD2",
                UseLists = true,
                UseNullableTypes = true,
                StripDebuggerStepThroughAttribute = true,
                ExcludeImportedTypes = true,
                MixedContent = true
            };
            
            using (var o = File.CreateText(@"Schemas\Form.cs"))
            {
                var generator = new XsdCodeGenerator() { Options = options };
                generator.Generate(new[] { @"Schemas\Form.xsd" }, o);
            }
        }

    }
}
