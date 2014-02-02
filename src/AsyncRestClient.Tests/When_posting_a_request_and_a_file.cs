using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using AsyncRestClient;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;

namespace SimpleAsyncClient.Tests
{
    [TestFixture]
    class When_posting_a_request_and_a_file
    {
        readonly Dictionary<string, string> dto = new Dictionary<string, string>();
        string fileContent;
        IHttpClient httpClient;
        RestRequest request;
        RestResponse response;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            httpClient = Substitute.For<IHttpClient>();
            httpClient.DefaultRequestHeaders.Returns(FormatterServices.GetUninitializedObject(typeof (HttpRequestHeaders)));
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("fileContent"));
            var asyncClient = new AsyncClient(httpClient, new JsonNetSerializer());
            var restResponse = new RestResponse {Address = "address"};
            httpClient.When(x => x.SendAsync(Arg.Any<HttpRequestMessage>())).Do(
                x =>
                {
                    fileContent =
                        Encoding.UTF8.GetString(
                            x.Arg<HttpRequestMessage>().Content.As<MultipartFormDataContent>().Last().ReadAsByteArrayAsync().Result);
                    var stringContents = x.Arg<HttpRequestMessage>().Content.As<MultipartFormDataContent>().OfType<StringContent>().ToList();
                    stringContents.ForEach(sc => dto.Add(sc.Headers.ContentDisposition.Name.Trim('"'), sc.ReadAsStringAsync().Result));
                });
            var responseMessage = new HttpResponseMessage {Content = new StringContent(JsonConvert.SerializeObject(restResponse))};
            Func<Task<HttpResponseMessage>> task = async () => responseMessage;
            httpClient.SendAsync(Arg.Any<HttpRequestMessage>()).Returns(task());
            request = new RestRequest {UserName = "userName"};
            response = asyncClient.Post<RestRequest, RestResponse>(request, "requestUri", "fileName", stream).Result;
        }

        [Test]
        public void Then_httpClient_is_called()
        {
            httpClient.Received().SendAsync(Arg.Is<HttpRequestMessage>(x => x.RequestUri.ToString() == "requestUri"));
            var json = JsonConvert.SerializeObject(request);
            var dictionary = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
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