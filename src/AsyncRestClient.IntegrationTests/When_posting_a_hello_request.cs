using AsyncRestClient.IntegrationTests.Service;
using FluentAssertions;
using NUnit.Framework;

namespace AsyncRestClient.IntegrationTests
{
    public class When_posting_a_hello_request
    {
        [Test]
        public void Then_response_should_be_hello_name()
        {
            var response = SetUpFixture.AsyncClient.Post<Hello, HelloResponse>(new Hello {Name = "world"}, "hello").Result;
            response.Reply.Should().Be("Hello, world");
        }
    }
}