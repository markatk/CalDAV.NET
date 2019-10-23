using System;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;

namespace CalDAV.NET.Extensions
{
    internal static class XDocumentExtensions
    {
        public static StringContent ToStringContent(this XDocument document)
        {
            return new StringContent(document.ToString(), Encoding.UTF8, "application/xml");
        }
    }
}
