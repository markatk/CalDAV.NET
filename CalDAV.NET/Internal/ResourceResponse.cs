using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CalDAV.NET.Internal.Extensions;

namespace CalDAV.NET.Internal
{
    internal class ResourceResponse : Response
    {
        public IReadOnlyCollection<Resource> Resources { get; }

        public ResourceResponse(string method, int statusCode, IReadOnlyCollection<Resource> resources) : base(method, statusCode)
        {
            Resources = resources;
        }

        public new static async Task<ResourceResponse> ParseAsync(HttpResponseMessage message)
        {
            var data = await message.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var content = GetEncoding(message.Content, Encoding.UTF8).GetString(data, 0, data.Length);

            var resources = new List<Resource>();
            if (TryParseDocument(content, out var document) && document?.Root != null)
            {
                resources = document.Root
                    .LocalNameElements("response")
                    .Select(ParseResource)
                    .ToList();
            }

            return new ResourceResponse(message.RequestMessage.Method.Method, (int) message.StatusCode, resources);
        }

        private static Resource ParseResource(XElement element)
        {
            var uri = element.LocalNameElement("href")?.Value;

            var properties = element
                .LocalNameElements("propstat")
                .Where(x =>
                {
                    var statusCode = x.GetStatusCode();

                    return statusCode >= 200 && statusCode <= 299;
                })
                .SelectMany(x => x.LocalNameElements("prop").Elements())
                .ToList();

            return new Resource(properties.Select(x => new KeyValuePair<XName, string>(x.Name, x.GetInnerXml())).ToDictionary(x => x.Key, x => x.Value));
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
