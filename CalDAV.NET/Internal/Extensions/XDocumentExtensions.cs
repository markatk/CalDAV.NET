using System;
using System.Xml.Linq;

namespace CalDAV.NET.Internal.Extensions
{
    internal static class XDocumentExtensions
    {
        public static string ToStringWithDeclaration(this XDocument document)
        {
            return document.Declaration + Environment.NewLine + document;
        }
    }
}
