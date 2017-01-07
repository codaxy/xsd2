using System;
using System.Collections.Generic;
using System.IO;

using Xsd2.Capitalizers;

namespace Xsd2
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                var options = new XsdCodeGeneratorOptions
                {
                    UseNullableTypes = false,
                    OutputNamespace = "Xsd2",
                    UseLists = false,
                    StripDebuggerStepThroughAttribute = false,
                    MixedContent = false,
                    ExcludeImportedTypes = false,
                    Imports = new List<string>(),
                    UsingNamespaces = new List<string>()
                };

                String outputDirectory = null;
                var combine = false;
                string outputFileName = null;
                var help = false;

                var optionSet = new Mono.Options.OptionSet()
                {
                    { "?|h|help", "Shows the help text", s => help = true },
                    { "o|out|output=", "Sets the output directory", s => outputDirectory = s },
                    { "l|language=", "Sets the language to use for code generation (CS or VB)", s => options.Language = (XsdCodeGeneratorOutputLanguage)Enum.Parse(typeof(XsdCodeGeneratorOutputLanguage), s, true) },
                    { "header", "Write file header", s => options.WriteFileHeader = s != null },
                    { "order", "Preserve the element order", s => options.PreserveOrder = s != null },
                    { "edb|enableDataBinding", "Implements INotifyPropertyChanged for all types", s => options.EnableDataBinding = s != null },
                    { "lists", "Use lists", s => options.UseLists = s != null },
                    { "strip-debug-attributes", "Strip debug attributes", s => options.StripDebuggerStepThroughAttribute = s != null },
                    { "pcl", "Target a PCL", s => options.StripPclIncompatibleAttributes = s != null },
                    { "cp|capitalize:", "Capitalize properties", s => options.PropertyNameCapitalizer = GetCapitalizer(s) },
                    { "ct|capitalize-types:", "Capitalize types", s => options.TypeNameCapitalizer = GetCapitalizer(s) },
                    { "ce|capitalize-enum-values:", "Capitalize enum values", s => options.EnumValueCapitalizer = GetCapitalizer(s) },
                    { "mixed", "Support mixed content", s => options.MixedContent = s != null },
                    { "n|ns|namespace=", "Sets the output namespace", s => options.OutputNamespace = s },
                    { "import=", "Adds import", s => options.Imports.Add(s) },
                    { "u|using=", "Adds a namespace to use", s => options.UsingNamespaces.Add(s) },
                    { "ei|exclude-imports", "Exclude imported types", s => options.ExcludeImportedTypes = s != null },
                    { "ein|exclude-imports-by-name", "Exclude imported types by name", s => options.ExcludeImportedTypes = options.ExcludeImportedTypesByNameAndNamespace = s != null },
                    { "nullable", "Use nullable types", s => options.UseNullableTypes = options.HideUnderlyingNullableProperties = s != null },
                    { "all", "Enable all flags", s =>
                    {
                        options.StripDebuggerStepThroughAttribute = options.UseLists = options.UseNullableTypes = options.ExcludeImportedTypes = options.MixedContent = s != null;
                        options.PropertyNameCapitalizer = new FirstCharacterCapitalizer();
                    } },
                    { "combine:", "Combine output to a single file", s => { combine = true; outputFileName = s; } },
                    { "c|classes", "Generates classes for the schema", s => { }, true },
                    { "nologo", "Suppresses application banner", s => { }, true },
                };

                var inputs = optionSet.Parse(args);
                if (help || args.Length == 0 || inputs.Count == 0)
                {
                    Console.Error.WriteLine("Xsd2 [options] schema.xsd ...");
                    optionSet.WriteOptionDescriptions(Console.Error);
                    return 1;
                }

                var generator = new XsdCodeGenerator() { Options = options };

                if (combine)
                {
                    String outputPath = null;
                    foreach (var path in inputs)
                    {
                        var fileInfo = new FileInfo(path);

                        if (outputPath == null)
                        {
                            if (string.IsNullOrEmpty(outputFileName))
                                outputFileName = Path.ChangeExtension(fileInfo.Name, ".cs");
                            outputPath = Path.Combine(outputDirectory ?? fileInfo.DirectoryName, outputFileName);
                        }

                        Console.WriteLine(fileInfo.FullName);
                    }

                    Console.WriteLine(outputPath);

                    using (var output = File.CreateText(outputPath))
                        generator.Generate(inputs, output);
                }
                else
                {
                    foreach (var path in inputs)
                    {
                        var fileInfo = new FileInfo(path);
                        var outputPath = Path.Combine(outputDirectory ?? fileInfo.DirectoryName, Path.ChangeExtension(fileInfo.Name, ".cs"));

                        Console.WriteLine(fileInfo.FullName);
                        Console.WriteLine(outputPath);

                        using (var output = File.CreateText(outputPath))
                            generator.Generate(new[] { path }, output);
                    }
                }         
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("XSD2 code generation failed.");
                Console.Error.Write(ex.ToString());
                return 2;
            }

            return 0;
        }

        private static ICapitalizer GetCapitalizer(string name)
        {
            if (string.IsNullOrEmpty(name))
                return new FirstCharacterCapitalizer();

            switch (name)
            {
                case "first-character":
                case "first-char":
                case "first":
                    return new FirstCharacterCapitalizer();
                case "none":
                    return new NoneCapitalizer();
            }
            
            throw new NotSupportedException(string.Format("There is no capitalizer associated with the name {0}", name));
        }
    }
}