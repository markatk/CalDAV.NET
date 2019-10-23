using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace CalDAV.NET.Extensions
{
    internal static class XElementExtensions
    {
        private static readonly Regex StatusCodeRegex = new Regex(@".*(\d{3}).*");

        public static XElement LocalNameElement(this XElement element, string localName, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            return element.Elements().FirstOrDefault(x => x.Name.LocalName.Equals(localName, comparison));
        }

        public static IEnumerable<XElement> LocalNameElements(this XElement element, string localName, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            return element.Elements().Where(x => x.Name.LocalName.Equals(localName, comparison));
        }

        public static string GetInnerXml(this XElement element)
        {
            using (var reader = element.CreateReader())
            {
                reader.MoveToContent();

                return reader.ReadInnerXml();
            }
        }

        public static int GetStatusCode(this XElement element)
        {
            var rawValue = element.LocalNameElement("status")?.Value;
            if (string.IsNullOrEmpty(rawValue))
            {
                return -1;
            }

            var codeGroup = StatusCodeRegex.Match(rawValue).Groups[1];
            if (codeGroup.Success == false)
            {
                return -1;
            }

            if (int.TryParse(codeGroup.Value, out var statusCode) == false)
            {
                return -1;
            }

            return statusCode;
        }

        public static string GetDescription(this XElement element)
        {
            return element.LocalNameElement("responsedescription")?.Value ?? element.LocalNameElement("status")?.Value;
        }
    }
}
