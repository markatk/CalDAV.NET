using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
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

        public Uri BaseUri { get; set; }

        public CalDAVClient()
        {
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            _client.DefaultRequestHeaders.Add("Depth", "1");
            _client.DefaultRequestHeaders.Add("Prefer", "return-minimal");
        }

        public async Task<ResourceResponse> GetAsync(string uri)
        {
            var message = await SendAsync(HttpMethod.Get, uri).ConfigureAwait(false);

            return await ResourceResponse.ParseAsync(message).ConfigureAwait(false);
        }

        public async Task<Response> PutAsync(string uri, string content)
        {
            var body = new StringContent(content, Encoding.UTF8, "text/calendar");

            var message = await SendAsync(HttpMethod.Put, uri, null, body).ConfigureAwait(false);

            return await Response.ParseAsync(message).ConfigureAwait(false);
        }

        public async Task<Response> DeleteAsync(string uri)
        {
            var message = await SendAsync(HttpMethod.Delete, uri).ConfigureAwait(false);

            return await Response.ParseAsync(message).ConfigureAwait(false);
        }

        public async Task<ResourceResponse> PropfindAsync(string uri, XDocument content = null)
        {
            var responseMessage = await SendAsync(new HttpMethod("PROPFIND"), uri, null, content?.ToStringContent()).ConfigureAwait(false);

            return await ResourceResponse.ParseAsync(responseMessage).ConfigureAwait(false);
        }

        public async Task<ResourceResponse> ReportAsync(string uri, XDocument content = null)
        {
            var responseMessage = await SendAsync(new HttpMethod("REPORT"), uri, null, content?.ToStringContent()).ConfigureAwait(false);

            return await ResourceResponse.ParseAsync(responseMessage).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> SendAsync(
            HttpMethod method,
            string uri,
            IReadOnlyDictionary<string, string> headers = null,
            HttpContent content = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var request = new HttpRequestMessage(method, new Uri(BaseUri, uri)))
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
