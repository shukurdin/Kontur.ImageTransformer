using System.IO;
using System.Net;

namespace Kontur.ImageTransformer.Requests
{
    public struct Request
    {
        public Request(string rawUrl, string httpMethod = "POST",
            long contentLength64 = -1, Stream inputStream = null)
        {
            RawUrl = rawUrl;
            HttpMethod = httpMethod;
            ContentLength64 = contentLength64;
            InputStream = inputStream;
        }

        public Request(HttpListenerRequest request)
        {
            RawUrl = request.RawUrl;
            HttpMethod = request.HttpMethod;
            ContentLength64 = request.ContentLength64;
            InputStream = request.InputStream;
        }

        public string RawUrl { get; }
        public string HttpMethod { get; }
        public long ContentLength64 { get; }
        public Stream InputStream { get; }
    }
}