using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using CalDAV.NET.Internal.Extensions;

namespace CalDAV.NET.Internal
{
    internal class CalDAVClient
    {
        private static readonly HttpClient _client = new HttpClient();
        private static readonly XNamespace _davNs = "DAV:";
        private static readonly XNamespace _calNs = "urn:ietf:params:xml:ns:caldav";

        public Uri BaseUri { get; set; }

        public CalDAVClient()
        {
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            _client.DefaultRequestHeaders.Add("Depth", "1");
            _client.DefaultRequestHeaders.Add("Prefer", "return-minimal");
        }

        public async Task<ResourceResponse> PropfindAsync(string uri)
        {
            // create body
            var propfind = new XElement(_davNs + "propfind", new XAttribute(XNamespace.Xmlns + "d", _davNs));
            propfind.Add(new XElement(_davNs + "allprop"));

            var document = new XDocument(new XDeclaration("1.0", "UTF-8", null));
            document.Add(propfind);

            // send request and parse response
            var responseMessage = await SendAsync("PROPFIND", new Uri(BaseUri, uri), null, document.ToStringContent()).ConfigureAwait(false);
            var response = await ResourceResponse.ParseAsync(responseMessage).ConfigureAwait(false);

            return response;
        }

        public async Task<ResourceResponse> ReportAsync(string uri)
        {
            // create body
            var query = new XElement(_calNs + "calendar-query", new XAttribute(XNamespace.Xmlns + "d", _davNs), new XAttribute(XNamespace.Xmlns + "c", _calNs));

            var prop = new XElement(_davNs + "prop");
            prop.Add(new XElement(_davNs + "getetag"));
            prop.Add(new XElement(_calNs + "calendar-data"));
            query.Add(prop);

            var filter = new XElement(_calNs + "filter");
            filter.Add(new XElement(_calNs + "comp-filter", new XAttribute("name", "VCALENDAR")));
            query.Add(filter);

            var document = new XDocument(new XDeclaration("1.0", "UTF-8", null));
            document.Add(query);

            // send request and parse response
            var responseMessage = await SendAsync("REPORT", new Uri(BaseUri, uri), null, document.ToStringContent()).ConfigureAwait(false);
            var response = await ResourceResponse.ParseAsync(responseMessage).ConfigureAwait(false);

            return response;
        }

        private async Task<HttpResponseMessage> SendAsync(
            string method,
            Uri uri,
            IReadOnlyDictionary<string, string> headers = null,
            HttpContent content = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var request = new HttpRequestMessage(new HttpMethod(method), uri))
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                request.Content = content;

                var response = await _client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false);

                return response;
            }
        }
    }
}
