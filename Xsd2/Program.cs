using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.CSharp;

namespace Xsd2
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var inputs = new List<String>();

                var options = new XsdCodeGeneratorOptions
                {
                    UseNullableTypes = true,
                    OutputNamespace = "Xsd2",
                    UseLists = true,
                    CapitalizeProperties = true,
                    StripDebuggerStepThroughAttribute = true
                };

                var generator = new XsdCodeGenerator() { Options = options };
                String outputDirectory = null;

                foreach (var arg in args)
                {
                    if (!arg.StartsWith("/"))
                        inputs.Add(arg);
                    else
                    {
                        var colonIndex = arg.IndexOf(':');
                        if (colonIndex == -1)
                            throw new InvalidOperationException("Invalid argument: " + arg);
                        
                        var option = arg.Substring(0, colonIndex + 1);
                        var value = arg.Substring(colonIndex + 1);
                        
                        switch (option.ToLower())
                        {
                            case "/o:":
                                outputDirectory = value;
                                break;
                            case "/ns:":
                            case "/namespace:":
                                options.OutputNamespace = value;
                                break;
                        }
                    }
                }

                foreach (var path in inputs)
                {
                    var fileInfo = new FileInfo(path);
                    var outputPath = Path.Combine(outputDirectory ?? fileInfo.DirectoryName, Path.ChangeExtension(fileInfo.Name, ".cs"));


                    Console.WriteLine(fileInfo.FullName);
                    Console.WriteLine(outputPath);

                    using (var input = fileInfo.OpenRead())
                    using (var output = File.CreateText(outputPath))
                        generator.Generate(input, output);
                }               
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("XSD2 code generation failed.");
                Console.Error.Write(ex.ToString());
            }
        }
    }
}