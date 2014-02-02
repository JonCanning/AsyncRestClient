using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using AsyncRestClient;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace SimpleAsyncClient.Tests
{
    [TestFixture]
    class When_calling_send_without_a_request_and_it_fails
    {
        IHttpClient httpClient;
        bool onExceptionCalled;
        bool requestHasFailed;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            httpClient = Substitute.For<IHttpClient>();
            httpClient.DefaultRequestHeaders.Returns(FormatterServices.GetUninitializedObject(typeof (HttpRequestHeaders)));
            var asyncClient = new AsyncClient(httpClient, new JsonNetSerializer())
            {
                OnUnsuccessful = x => requestHasFailed = true,
                OnException = x => onExceptionCalled = true
            };
            var responseMessage = new HttpResponseMessage {StatusCode = HttpStatusCode.BadRequest};
            Func<Task<HttpResponseMessage>> task = async () => responseMessage;
            httpClient.SendAsync(Arg.Any<HttpRequestMessage>()).Returns(task());
            var response = asyncClient.Send<RestResponse>(HttpMethod.Get, "requestUri").Result;
        }

        [Test]
        public void Then_OnException_is_not_called()
        {
            onExceptionCalled.Should().BeFalse();
        }

        [Test]
        public void Then_OnUnsuccessful_is_called()
        {
            requestHasFailed.Should().BeTrue();
        }

        [Test]
        public void Then_httpClient_is_called()
        {
            httpClient.Received()
                .SendAsync(
                    Arg.Is<HttpRequestMessage>(x => x.RequestUri.ToString() == "requestUri" && x.Method == HttpMethod.Get && x.Content == null));
        }
    }
}