namespace Xsd2.Capitalizers
{
    public class FirstCharacterCapitalizer : ICapitalizer
    {
        public string Capitalize(string name)
        {
            if (!char.IsLower(name[0]))
                return name;

            return name.Substring(0, 1).ToUpper() + name.Substring(1);
        }
    }
}
