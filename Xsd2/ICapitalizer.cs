using System.CodeDom;

namespace Xsd2
{
    public interface ICapitalizer
    {
        string Capitalize(CodeNamespace codeNamespace, CodeTypeMember member);
    }
}
