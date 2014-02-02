using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AsyncRestClient
{
    public class AsyncClient : IAsyncClient
    {
        readonly IHttpClient httpClient;
        readonly ISerializer serializer;

        public AsyncClient(Uri baseAddress) : this(new HttpClient {BaseAddress = baseAddress}, new JsonNetSerializer()) {}

        public AsyncClient(Uri baseAddress, ISerializer serializer) : this(new HttpClient {BaseAddress = baseAddress}, serializer) {}

        public AsyncClient(IHttpClient httpClient, ISerializer serializer)
        {
            this.httpClient = httpClient;
            this.serializer = serializer;
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(serializer.ContentType));
            OnBeforeSend = delegate { };
            OnUnsuccessful = delegate { };
            OnAfterSend = delegate { };
            OnException = delegate { };
        }

        public Action<HttpRequestMessage> OnBeforeSend { get; set; }
        public Action OnAfterSend { get; set; }
        public Action<HttpResponseMessage> OnUnsuccessful { get; set; }
        public Action<Exception> OnException { get; set; }

        public async Task Delete(string requestUri)
        {
            await Send(HttpMethod.Delete, requestUri);
        }

        public async Task<TResponse> Delete<TResponse>(string requestUri) where TResponse : class
        {
            return await Send<TResponse>(HttpMethod.Delete, requestUri);
        }

        public async Task<TResponse> Get<TResponse>(string requestUri) where TResponse : class
        {
            return await Send<TResponse>(HttpMethod.Get, requestUri);
        }

        public async Task<TResponse> Post<TRequest, TResponse>(TRequest request, string requestUri) where TRequest : class where TResponse : class
        {
            return await Send<TRequest, TResponse>(request, HttpMethod.Post, requestUri);
        }

        public async Task<TResponse> Put<TRequest, TResponse>(TRequest request, string requestUri) where TRequest : class where TResponse : class
        {
            return await Send<TRequest, TResponse>(request, HttpMethod.Put, requestUri);
        }

        public async Task<TResponse> Send<TRequest, TResponse>(TRequest request, HttpMethod httpMethod, string requestUri) where TRequest : class
            where TResponse : class
        {
            var content = new StringContent(serializer.Serialize(request), Encoding.UTF8, serializer.ContentType);
            return await Send<TResponse>(httpMethod, requestUri, content);
        }

        public async Task<TResponse> Send<TResponse>(HttpMethod httpMethod, string requestUri, HttpContent content = null) where TResponse : class
        {
            using (var httpRequestMessage = new HttpRequestMessage(httpMethod, requestUri))
            {
                httpRequestMessage.Content = content;
                return await Send<TResponse>(httpRequestMessage);
            }
        }

        public async Task<TResponse> Post<TRequest, TResponse>(TRequest request, string requestUri, string fileName, Stream stream)
            where TRequest : class where TResponse : class
        {
            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri))
            using (var multipartFormDataContent = new MultipartFormDataContent())
            {
                var json = serializer.Serialize(request);
                var dictionary = serializer.Deserialize<IDictionary<string, object>>(json);
                foreach (var kvp in dictionary)
                {
                    var stringContent = new StringContent(kvp.Value.ToString());
                    multipartFormDataContent.Add(stringContent, "\"" + kvp.Key + "\"");
                }
                using (stream)
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    multipartFormDataContent.Add(new ByteArrayContent(memoryStream.ToArray()), fileName, "\"" + fileName + "\"");
                }
                httpRequestMessage.Content = multipartFormDataContent;
                return await Send<TResponse>(httpRequestMessage);
            }
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        public async Task Send(HttpMethod httpMethod, string requestUri)
        {
            using (var httpRequestMessage = new HttpRequestMessage(httpMethod, requestUri))
                await Send(httpRequestMessage);
        }

        async Task<TResponse> Send<TResponse>(HttpRequestMessage httpRequestMessage) where TResponse : class
        {
            OnBeforeSend(httpRequestMessage);
            TResponse response = null;
            try
            {
                using (var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage))
                    if (!httpResponseMessage.IsSuccessStatusCode)
                        OnUnsuccessful(httpResponseMessage);
                    else
                    {
                        var content = await httpResponseMessage.Content.ReadAsStringAsync();
                        response = serializer.Deserialize<TResponse>(content);
                        return response;
                    }
            }
            catch (Exception exception)
            {
                OnException(exception);
            }
            finally
            {
                OnAfterSend();
            }
            return response;
        }

        async Task Send(HttpRequestMessage httpRequestMessage)
        {
            OnBeforeSend(httpRequestMessage);
            try
            {
                using (var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage))
                    if (!httpResponseMessage.IsSuccessStatusCode)
                        OnUnsuccessful(httpResponseMessage);
            }
            catch (Exception exception)
            {
                OnException(exception);
            }
            finally
            {
                OnAfterSend();
            }
        }
    }
}