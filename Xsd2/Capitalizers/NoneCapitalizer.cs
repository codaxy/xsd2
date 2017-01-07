using System.CodeDom;

namespace Xsd2.Capitalizers
{
    public class NoneCapitalizer : ICapitalizer
    {
        public string Capitalize(CodeNamespace codeNamespace, CodeTypeMember member)
        {
            return member.Name;
        }
    }
}
