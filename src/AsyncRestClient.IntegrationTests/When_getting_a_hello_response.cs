using AsyncRestClient.IntegrationTests.Service;
using FluentAssertions;
using NUnit.Framework;

namespace AsyncRestClient.IntegrationTests
{
    public class When_getting_a_hello_response
    {
        [Test]
        public void Then_response_should_be_hello()
        {
            var response = SetUpFixture.AsyncClient.Get<HelloResponse>("hello/world").Result;
            response.Reply.Should().Be("Hello, world");
        }
    }
}