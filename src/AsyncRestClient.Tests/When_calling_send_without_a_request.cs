using System.Linq;
using System.Net.Http;
using FluentAssertions;
using NUnit.Framework;

namespace AsyncRestClient.Tests
{
    [TestFixture]
    class When_calling_send_without_a_request : AsyncClientFixture
    {
        RestResponse response;

        public When_calling_send_without_a_request()
        {
            HttpClient.AddResponses(JsonNetSerializer.Serialize(new RestResponse {Address = "address"}));
            response = AsyncClient.Send<RestResponse>(HttpMethod.Get, "requestUri").Result;
        }

        [Test]
        public void Then_httpClient_is_called()
        {
            var httpRequestMessage = HttpClient.Requests.Single();
            httpRequestMessage.RequestUri.Should().Be("requestUri");
            httpRequestMessage.Method.Should().Be(HttpMethod.Get);
            RequestContent.Should().Be(null);
        }

        [Test]
        public void Then_response_is_deserialized()
        {
            response.Address.Should().Be("address");
        }
    }
}