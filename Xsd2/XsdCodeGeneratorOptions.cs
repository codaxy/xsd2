using System;
using System.Collections.Generic;

namespace Xsd2
{
    public class XsdCodeGeneratorOptions
    {
        public bool UseLists { get; set; }
        public bool CapitalizeProperties { get; set; }
        public bool CapitalizeEnumValues { get; set; }
        public bool StripDebuggerStepThroughAttribute { get; set; }
        public bool StripPclIncompatibleAttributes { get; set; }
        public bool HideUnderlyingNullableProperties { get; set; }
        public bool UseNullableTypes { get; set; }
        public List<String> Imports { get; set; }
        public List<String> UsingNamespaces { get; set; }

        public string OutputNamespace { get; set; }
        public bool WriteFileHeader { get; set; }
        public bool PreserveOrder { get; set; }
        public bool EnableDataBinding { get; set; }

        public bool ExcludeImportedTypes { get; set; }

        public bool ExcludeImportedTypesByNameAndNamespace { get; set; }

        public bool MixedContent { get; set; }
    }
}
