using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;

namespace CalDAV.NET.Internal
{
    internal class CalDAVClient
    {
        private static readonly HttpClient _client = new HttpClient();
        private static readonly HttpMethod _propfindMethod = new HttpMethod("PROPFIND");
        private static readonly HttpMethod _reportMethod = new HttpMethod("REPORT");

        public Uri BaseUri { get; set; }

        public CalDAVClient()
        {
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            _client.DefaultRequestHeaders.Add("Prefer", "return-minimal");
            _client.DefaultRequestHeaders.Add("Depth", "1");
        }

        public Request<ResourceResponse> Get(string uri)
        {
            return CreateRequest<ResourceResponse>(uri)
                .WithMethod(HttpMethod.Get);
        }

        public Request<Response> Put(string uri, string content)
        {
            return CreateRequest<Response>(uri)
                .WithMethod(HttpMethod.Put)
                .WithContent(new StringContent(content));
        }

        public Request<Response> Delete(string uri)
        {
            return CreateRequest<Response>(uri)
                .WithMethod(HttpMethod.Delete);
        }

        public Request<ResourceResponse> Propfind(string uri, XElement root)
        {
            return CreateRequest<ResourceResponse>(uri)
                .WithMethod(_propfindMethod)
                .WithXmlContent(root);
        }

        public Request<ResourceResponse> Report(string uri, XElement root)
        {
            return CreateRequest<ResourceResponse>(uri)
                .WithMethod(_reportMethod)
                .WithXmlContent(root);
        }

        internal void SetAuthorization(string username, string password)
        {
            var value = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", value);
        }

        private Request<T> CreateRequest<T>(string uri) where T : Response, new()
        {
            return new Request<T>(new Uri(BaseUri, uri), _client);
        }
    }
}
