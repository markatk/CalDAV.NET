using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CalDAV.NET.Internal
{
    internal class Response
    {
        public int StatusCode { get; private set; }
        public string Method { get; private set; }

        public Response()
        {
        }

        public Response(string method, int statusCode)
        {
            Method = method;
            StatusCode = statusCode;
        }

        public virtual bool IsSuccessful => StatusCode >= 200 && StatusCode <= 299;

        public virtual Task ParseAsync(HttpResponseMessage message)
        {
            Method = message.RequestMessage.Method.Method;
            StatusCode = (int) message.StatusCode;

            return Task.CompletedTask;
        }

        public override string ToString()
        {
            return $"${Method} CalDAV response - Status code: {StatusCode}";
        }

        protected static Encoding GetEncoding(HttpContent content, Encoding defaultEncoding)
        {
            if (content.Headers.ContentType?.CharSet == null)
            {
                return defaultEncoding;
            }

            try
            {
                return Encoding.GetEncoding(content.Headers.ContentType.CharSet);
            }
            catch
            {
                return defaultEncoding;
            }
        }
    }
}
