using System.Collections.Generic;
using System.Xml.Linq;

namespace CalDAV.NET.Internal
{
    internal class Resource
    {
        public IReadOnlyDictionary<XName, string> Properties { get; }

        public Resource(IReadOnlyDictionary<XName, string> properties)
        {
            Properties = properties;
        }
    }
}
