using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace AsyncRestClient.Tests
{
    [TestFixture]
    class When_posting_a_request_and_a_file : AsyncClientFixture
    {
        readonly Dictionary<string, string> dto = new Dictionary<string, string>();
        string fileContent;
        RestRequest request;
        RestResponse response;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("fileContent"));
            HttpClient.AddResponses(JsonNetSerializer.Serialize(new RestResponse { Address = "address" }));
            AsyncClient.OnBeforeSend = r =>
            {
                fileContent = Encoding.UTF8.GetString(r.Content.As<MultipartFormDataContent>().Last().ReadAsByteArrayAsync().Result);
                var stringContents = r.Content.As<MultipartFormDataContent>().OfType<StringContent>().ToList();
                stringContents.ForEach(sc => dto.Add(sc.Headers.ContentDisposition.Name.Trim('"'), sc.ReadAsStringAsync().Result));
            };
            HttpClient.AddResponses(JsonNetSerializer.Serialize(response));
            request = new RestRequest {UserName = "userName"};
            response = AsyncClient.Post<RestRequest, RestResponse>(request, "requestUri", "fileName", stream).Result;
        }

        [Test]
        public void Then_httpClient_is_called()
        {
            var json = JsonConvert.SerializeObject(request);
            var dictionary = JsonNetSerializer.Deserialize<IDictionary<string, object>>(json);
            foreach (var kvp in dictionary)
                dto[kvp.Key].Should().Be(kvp.Value.ToString());
            fileContent.Should().Be("fileContent");
        }

        [Test]
        public void Then_response_is_deserialized()
        {
            response.Address.Should().Be("address");
        }
    }
}