using System;
using System.Collections.Generic;
using System.Net.Http;
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

        public async Task<ResourceResponse> PropfindAsync(Uri uri)
        {
            var headers = new Dictionary<string, string>
            {
                { "Depth", "0"}
            };

            // create body
            var propfind = new XElement("{DAV:}propfind", new XAttribute(XNamespace.Xmlns + "D", "DAV:"));
            propfind.Add(new XElement("{DAV:}allprop"));

            var document = new XDocument(new XDeclaration("1.0", "UTF-8", null));
            document.Add(propfind);

            var content = new StringContent(document.ToStringWithDeclaration());

            // send request and parse response
            var responseMessage = await SendAsync("PROPFIND", uri, headers, content).ConfigureAwait(false);
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
