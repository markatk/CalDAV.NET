using System.Collections.Generic;
using System.Xml.Linq;

namespace CalDAV.NET
{
    internal class Resource
    {
        public IReadOnlyDictionary<XName, string> Properties { get; set; }
        public string Status { get; set; }
        public string Uri { get; set; }
    }
}
