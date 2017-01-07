namespace Xsd2.Capitalizers
{
    public class NoneCapitalizer : ICapitalizer
    {
        public string Capitalize(string name)
        {
            return name;
        }
    }
}
