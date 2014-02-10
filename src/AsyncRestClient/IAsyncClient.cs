using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace AsyncRestClient
{
    public interface IAsyncClient : IDisposable
    {
        Action<HttpRequestMessage> OnBeforeSend { get; set; }
        Action OnAfterSend { get; set; }
        Action<HttpResponseMessage> OnUnsuccessful { get; set; }
        Action<Exception> OnException { get; set; }
        Task Delete(string requestUri);
        Task<TResponse> Delete<TResponse>(string requestUri) where TResponse : class;
        Task<TResponse> Get<TResponse>(string requestUri) where TResponse : class;
        Task<TResponse> Post<TRequest, TResponse>(TRequest request, string requestUri) where TRequest : class where TResponse : class;
        Task<TResponse> Put<TRequest, TResponse>(TRequest request, string requestUri) where TRequest : class where TResponse : class;
        Task<TResponse> Send<TRequest, TResponse>(TRequest request, HttpMethod httpMethod, string requestUri) where TRequest : class
            where TResponse : class;
        Task<TResponse> Send<TResponse>(HttpMethod httpMethod, string requestUri, HttpContent content = null) where TResponse : class;
        Task<TResponse> Post<TRequest, TResponse>(TRequest request, string requestUri, string fileName, Stream stream) where TRequest : class
            where TResponse : class;
    }
}