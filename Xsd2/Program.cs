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
                    UseNullableTypes = false,
                    OutputNamespace = "Xsd2",
                    UseLists = false,
                    CapitalizeProperties = false,
                    StripDebuggerStepThroughAttribute = false,
                    Imports = new List<string>()
                };

                var generator = new XsdCodeGenerator() { Options = options };
                String outputDirectory = null;

                foreach (var arg in args)
                {
                    if (!arg.StartsWith("/"))
                        inputs.Add(arg);
                    else
                    {
                        String option, value;
                        var colonIndex = arg.IndexOf(':');
                        if (colonIndex == -1)
                        {
                            option = arg;
                            value = null;
                        }
                        else
                        {
                            option = arg.Substring(0, colonIndex + 1);
                            value = arg.Substring(colonIndex + 1);
                        }

                        switch (option.ToLower())
                        {
                            case "/o:":
                            case "/output:":
                                outputDirectory = value;
                                break;
                            case "/lists":
                                options.UseLists = true;
                                break;

                            case "/strip-debug-attributes":
                                options.StripDebuggerStepThroughAttribute = true;
                                break;

                            case "/capitalize":
                                options.CapitalizeProperties = true;
                                break;

                            case "/ns:":
                            case "/namespace:":
                                options.OutputNamespace = value;
                                break;

                            case "/import:":
                                options.Imports.Add(value);
                                break;

                            case "/all":
                                options.CapitalizeProperties = true;
                                options.StripDebuggerStepThroughAttribute = true;
                                options.UseLists = true;
                                options.UseNullableTypes = true;
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