using System;
using System.CodeDom;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Xsd2.Capitalizers
{
    public class WordCapitalizer : ICapitalizer
    {
        private static readonly Regex _wordRegEx = new Regex("\\w+", RegexOptions.Compiled);

        private readonly int _capitalizeAllUpperMinLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="WordCapitalizer"/> class.
        /// </summary>
        /// <remarks>
        /// This sets the minimum length for all-upper-case words to 3 before they get
        /// capitalizes. This is the standard as used by Microsoft. <see cref="System.IO"/>
        /// and <see cref="System.Xml"/>
        /// </remarks>
        public WordCapitalizer()
            : this(3)
        {
        }

        public WordCapitalizer(int capitalizeAllUpperMinLength)
        {
            _capitalizeAllUpperMinLength = capitalizeAllUpperMinLength;
        }

        public string Capitalize(CodeNamespace codeNamespace, CodeTypeMember member)
        {
            var name = member.GetOriginalName();
            if (name == member.Name)
            {
                if (!member.IsAnonymousType())
                {
                    name = member.Name;
                }
                else
                {
                    name = codeNamespace.GetNamesFromItems(member.Name).FirstOrDefault();
                    if (name == null)
                        name = member.Name;
                }
            }

            var matches = _wordRegEx.Matches(name);
            if (matches.Count == 0)
                return member.Name;
            var nameParts = matches.Cast<Match>().Select(x => x.Value);
            var result = new StringBuilder();
            foreach (var namePart in nameParts)
            {
                var isAllUpperCase = namePart.All(char.IsUpper);
                if (isAllUpperCase && namePart.Length >= _capitalizeAllUpperMinLength)
                {
                    result.Append(namePart.Substring(0, 1)).Append(namePart.Substring(1).ToLower());
                }
                else
                {
                    result.Append(namePart.Substring(0, 1).ToUpper()).Append(namePart.Substring(1));
                }
            }

            var resultName = result.ToString();
            if (!char.IsLetter(resultName[0]))
                return member.Name;
            return resultName;
        }
    }
}
