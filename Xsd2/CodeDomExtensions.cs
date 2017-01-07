using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace Xsd2
{
    internal static class CodeDomExtensions
    {
        private static readonly HashSet<string> _attributesWithNames = new HashSet<string>()
        {
            "System.Xml.Serialization.XmlAttributeAttribute",
            "System.Xml.Serialization.XmlElementAttribute",
            "System.Xml.Serialization.XmlArrayItemAttribute",
            "System.Xml.Serialization.XmlEnumAttribute",
            "System.Xml.Serialization.XmlRootAttribute",
            "System.Xml.Serialization.XmlTypeAttribute"
        };

        public static bool IsNameArgument(this CodeAttributeArgument argument)
        {
            if (string.IsNullOrEmpty(argument.Name))
            {
                var expr = argument.Value as CodePrimitiveExpression;
                if (expr == null)
                    return false;
                return expr.Value is string;
            }

            return argument.Name == "Name";
        }

        public static bool IsAttributeWithName(this CodeAttributeDeclaration attribute)
        {
            return _attributesWithNames.Contains(attribute.Name);
        }

        public static string GetOriginalName(this CodeTypeMember member)
        {
            if (member.Name == "Items")
                return member.Name;

            foreach (CodeAttributeDeclaration attribute in member.CustomAttributes)
            {
                if (!attribute.IsAttributeWithName())
                    continue;
                var nameArgument = attribute.Arguments.Cast<CodeAttributeArgument>().FirstOrDefault(x => x.IsNameArgument());
                if (nameArgument != null)
                    return (string)((CodePrimitiveExpression)nameArgument.Value).Value;
            }

            return member.Name;
        }

        public static bool IsAnonymousType(this CodeTypeMember member)
        {
            var attribute = member
                .CustomAttributes
                .Cast<CodeAttributeDeclaration>()
                .FirstOrDefault(x => x.Name == "System.Xml.Serialization.XmlTypeAttribute");
            if (attribute == null)
                return false;

            return attribute.IsAnonymousTypeArgument();
        }

        public static bool IsAnonymousTypeArgument(this CodeAttributeDeclaration attribute)
        {
            var anonymousTypeArgument = attribute
                .Arguments
                .Cast<CodeAttributeArgument>()
                .FirstOrDefault(x => x.Name == "AnonymousType");
            return anonymousTypeArgument != null;
        }

        public static string GetNamespace(this CodeTypeMember member)
        {
            var attribute = member
                .CustomAttributes
                .Cast<CodeAttributeDeclaration>()
                .FirstOrDefault(x => x.Name == "System.Xml.Serialization.XmlTypeAttribute");
            if (attribute == null)
                return null;

            var namespaceArgument = attribute
                .Arguments
                .Cast<CodeAttributeArgument>()
                .FirstOrDefault(x => x.Name == "Namespace");
            if (namespaceArgument == null)
                return string.Empty;

            return (string) ((CodePrimitiveExpression) namespaceArgument.Value).Value;
        }

        public static IEnumerable<string> GetNamesFromItems(this CodeNamespace codeNamespace, string typeName)
        {
            foreach (CodeTypeDeclaration codeType in codeNamespace.Types)
            {
                foreach (CodeTypeMember member in codeType.Members)
                {
                    if (member.Name != "Items")
                        continue;

                    foreach (CodeAttributeDeclaration attribute in member.CustomAttributes)
                    {
                        if (attribute.Name != "System.Xml.Serialization.XmlElementAttribute")
                            continue;

                        var itemName = (string)((CodePrimitiveExpression)attribute.Arguments[0].Value).Value;
                        foreach (CodeAttributeArgument argument in attribute.Arguments)
                        {
                            var typeOfExpr = argument.Value as CodeTypeOfExpression;
                            if (typeOfExpr != null)
                            {
                                if (!string.IsNullOrEmpty(typeOfExpr.Type.BaseType) && typeOfExpr.Type.BaseType == typeName)
                                    yield return itemName;

                                if (typeOfExpr.Type.ArrayElementType != null && typeOfExpr.Type.ArrayElementType.BaseType == typeName)
                                    yield return itemName;
                            }
                        }
                    }
                }
            }
        }
    }
}
