using System.Net.Http;

namespace AsyncRestClient
{
    public class HttpClient : System.Net.Http.HttpClient, IHttpClient
    {
        public HttpClient() {}

        public HttpClient(HttpMessageHandler handler) : base(handler) {}

        public HttpClient(HttpMessageHandler handler, bool disposeHandler) : base(handler, disposeHandler) {}
    }
}