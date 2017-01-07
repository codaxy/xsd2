using System.CodeDom;

namespace Xsd2.Capitalizers
{
    public class FirstCharacterCapitalizer : ICapitalizer
    {
        public string Capitalize(CodeNamespace codeNamespace, CodeTypeMember member)
        {
            var name = member.Name;

            if (!char.IsLower(name[0]))
                return name;

            return name.Substring(0, 1).ToUpper() + name.Substring(1);
        }
    }
}
