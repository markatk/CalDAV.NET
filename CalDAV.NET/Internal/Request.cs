using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using CalDAV.NET.Extensions;

namespace CalDAV.NET.Internal
{
    internal class Request<T> where T : Response, new()
    {
        public T Response { get; private set; }
        public HttpRequestHeaders Headers => _request.Headers;

        public HttpMethod Method
        {
            get => _request.Method;
            private set => _request.Method = value;
        }

        public HttpContent Content
        {
            get => _request.Content;
            private set => _request.Content = value;
        }

        private readonly HttpClient _client;
        private readonly HttpRequestMessage _request = new HttpRequestMessage();

        public Request(Uri uri, HttpClient client)
        {
            _request.RequestUri = uri;
            _client = client;
        }

        public async Task<T> SendAsync(CancellationToken cancellationToken = default)
        {
            var message = await _client.SendAsync(_request, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false);

            Response = new T();
            await Response.ParseAsync(message).ConfigureAwait(false);

            return Response;
        }

        public Request<T> WithMethod(HttpMethod method)
        {
            Method = method;

            return this;
        }

        public Request<T> WithContent(HttpContent content)
        {
            Content = content;

            return this;
        }

        public Request<T> WithXmlContent(XElement root)
        {
            var document = new XDocument(new XDeclaration("1.0", "UTF-8", null));
            document.Add(root);

            return WithContent(document.ToStringContent());
        }

        public Request<T> WithHeader(string name, string value)
        {
            Headers.Add(name, value);

            return this;
        }
    }
}
