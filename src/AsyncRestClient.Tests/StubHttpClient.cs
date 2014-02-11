using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace AsyncRestClient.Tests
{
    class StubHttpClient : IHttpClient
    {
        public readonly Queue<HttpRequestMessage> Requests = new Queue<HttpRequestMessage>();
        readonly Queue<string> responses = new Queue<string>();
        public HttpStatusCode HttpStatusCode { get; set; }

        public StubHttpClient()
        {
            DefaultRequestHeaders = FormatterServices.GetUninitializedObject(typeof(HttpRequestHeaders)) as HttpRequestHeaders;
            HttpStatusCode = HttpStatusCode.OK;
        }

        public void AddResponses(params string[] responses)
        {
            responses.ToList().ForEach(x => this.responses.Enqueue(x));
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            Requests.Enqueue(request);
            var result = responses.Any() ? responses.Dequeue() : string.Empty;
            return Task.Factory.StartNew(() => new HttpResponseMessage { Content = new StringContent(result), StatusCode = HttpStatusCode});
        }

        public void Dispose()
        {
        }

        public HttpRequestHeaders DefaultRequestHeaders { get; private set; }
    }
}