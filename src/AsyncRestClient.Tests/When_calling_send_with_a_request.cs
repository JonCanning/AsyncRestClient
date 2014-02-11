using System.Linq;
using System.Net.Http;
using FluentAssertions;
using NUnit.Framework;

namespace AsyncRestClient.Tests
{
    [TestFixture]
    class When_calling_send_with_a_request : AsyncClientFixture
    {
        readonly string requestJson;
        readonly RestResponse response;

        public When_calling_send_with_a_request()
        {
            HttpClient.AddResponses(JsonNetSerializer.Serialize(new RestResponse {Address = "address"}));
            var request = new RestRequest {UserName = "userName"};
            requestJson = JsonNetSerializer.Serialize(request);
            response = AsyncClient.Send<RestRequest, RestResponse>(request, HttpMethod.Post, "requestUri").Result;
        }

        [Test]
        public void Then_httpClient_is_called()
        {
            var httpRequestMessage = HttpClient.Requests.Single();
            httpRequestMessage.RequestUri.Should().Be("requestUri");
            httpRequestMessage.Method.Should().Be(HttpMethod.Post);
            RequestContent.Should().Be(requestJson);
        }

        [Test]
        public void Then_response_is_deserialized()
        {
            response.Address.Should().Be("address");
        }
    }
} ;