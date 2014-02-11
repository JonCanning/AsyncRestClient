using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace AsyncRestClient.Tests
{
    [TestFixture]
    class When_calling_send_without_a_request_and_it_fails : AsyncClientFixture
    {
        bool onExceptionCalled;
        bool onUnsuccessfulCalled;


        public When_calling_send_without_a_request_and_it_fails()
        {
            AsyncClient.OnUnsuccessful = x => onUnsuccessfulCalled = true;
            AsyncClient.OnException = x => onExceptionCalled = true;
            HttpClient.HttpStatusCode = HttpStatusCode.BadRequest;
            Task.WaitAll(AsyncClient.Send<RestResponse>(HttpMethod.Get, "requestUri"));
        }

        [Test]
        public void Then_OnException_is_not_called()
        {
            onExceptionCalled.Should().BeFalse();
        }

        [Test]
        public void Then_OnUnsuccessful_is_called()
        {
            onUnsuccessfulCalled.Should().BeTrue();
        }

        [Test]
        public void Then_httpClient_is_called()
        {
            var httpRequestMessage = HttpClient.Requests.Single();
            httpRequestMessage.RequestUri.Should().Be("requestUri");
            httpRequestMessage.Method.Should().Be(HttpMethod.Get);
            RequestContent.Should().Be(null);
        }
    }
}