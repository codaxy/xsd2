using System;
using System.Collections.Generic;

using Xsd2.Capitalizers;

namespace Xsd2
{
    public class XsdCodeGeneratorOptions
    {
        public XsdCodeGeneratorOptions()
        {
            Language = XsdCodeGeneratorOutputLanguage.CS;
            AttributesToRemove = new HashSet<string>();
        }

        public ICapitalizer PropertyNameCapitalizer { get; set; }
        public ICapitalizer EnumValueCapitalizer { get; set; }
        public ICapitalizer TypeNameCapitalizer { get; set; }
        public HashSet<string> AttributesToRemove { get; set; }
        public bool UseXLinq { get; set; }
        public bool UseLists { get; set; }
        public bool StripDebuggerStepThroughAttribute { get; set; }
        public bool HideUnderlyingNullableProperties { get; set; }
        public bool UseNullableTypes { get; set; }
        public List<String> Imports { get; set; }
        public List<String> UsingNamespaces { get; set; }

        public XsdCodeGeneratorOutputLanguage Language { get; set; }
        public string OutputNamespace { get; set; }
        public bool WriteFileHeader { get; set; }
        public bool PreserveOrder { get; set; }
        public bool EnableDataBinding { get; set; }

        public bool ExcludeImportedTypes { get; set; }

        public bool ExcludeImportedTypesByNameAndNamespace { get; set; }

        public bool MixedContent { get; set; }
    }
}
