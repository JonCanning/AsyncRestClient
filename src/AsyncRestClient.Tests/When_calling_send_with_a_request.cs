using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using AsyncRestClient;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;

namespace SimpleAsyncClient.Tests
{
    [TestFixture]
    class When_calling_send_with_a_request
    {
        string content;
        IHttpClient httpClient;
        RestRequest request;
        RestResponse response;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            httpClient = Substitute.For<IHttpClient>();
            httpClient.DefaultRequestHeaders.Returns(FormatterServices.GetUninitializedObject(typeof (HttpRequestHeaders)));
            var asyncClient = new AsyncClient(httpClient, new JsonNetSerializer());
            var restResponse = new RestResponse {Address = "address"};
            httpClient.When(x => x.SendAsync(Arg.Any<HttpRequestMessage>()))
                .Do(x => content = x.Arg<HttpRequestMessage>().Content.ReadAsStringAsync().Result);
            var responseMessage = new HttpResponseMessage {Content = new StringContent(JsonConvert.SerializeObject(restResponse))};
            Func<Task<HttpResponseMessage>> task = async () => responseMessage;
            httpClient.SendAsync(Arg.Any<HttpRequestMessage>()).Returns(task());
            request = new RestRequest {UserName = "userName"};
            response = asyncClient.Send<RestRequest, RestResponse>(request, HttpMethod.Post, "requestUri").Result;
        }

        [Test]
        public void Then_httpClient_is_called()
        {
            httpClient.Received()
                .SendAsync(
                    Arg.Is<HttpRequestMessage>(
                        x => x.RequestUri.ToString() == "requestUri" && x.Method == HttpMethod.Post && content == JsonConvert.SerializeObject(request)));
        }

        [Test]
        public void Then_response_is_deserialized()
        {
            response.Address.Should().Be("address");
        }
    }
}