using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CalDAV.NET.Extensions;

namespace CalDAV.NET.Internal
{
    internal class ResourceResponse : Response
    {
        public IReadOnlyCollection<Resource> Resources { get; private set; }

        public ResourceResponse()
        {
            Resources = new List<Resource>();
        }

        public ResourceResponse(string method, int statusCode, IReadOnlyCollection<Resource> resources) : base(method, statusCode)
        {
            Resources = resources;
        }

        public override async Task ParseAsync(HttpResponseMessage message)
        {
            await base.ParseAsync(message);

            var data = await message.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var content = GetEncoding(message.Content, Encoding.UTF8).GetString(data, 0, data.Length);

            if (TryParseDocument(content, out var document) == false || document.Root == null)
            {
                return;
            }

            Resources = document.Root
                .LocalNameElements("response")
                .Select(ParseResource)
                .ToList();
        }

        private static Resource ParseResource(XElement element)
        {
            var uri = element.LocalNameElement("href")?.Value;
            var status = element.LocalNameElement("status")?.Value;

            var properties = element
                .LocalNameElements("propstat")
                .Where(x =>
                {
                    var statusCode = x.GetStatusCode();

                    return statusCode >= 200 && statusCode <= 299;
                })
                .SelectMany(x => x.LocalNameElements("prop").Elements())
                .Select(x => new KeyValuePair<XName, string>(x.Name, x.GetInnerXml()))
                .ToDictionary(x => x.Key, x => x.Value);

            return new Resource
            {
                Uri = uri,
                Status = status,
                Properties = properties
            };
        }

        private static bool TryParseDocument(string text, out XDocument document)
        {
            try
            {
                document = XDocument.Parse(text);

                return true;
            }
            catch
            {
                document = null;
            }

            return false;
        }
    }
}
