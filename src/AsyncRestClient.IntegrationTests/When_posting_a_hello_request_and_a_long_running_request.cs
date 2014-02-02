using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using AsyncRestClient.IntegrationTests.Service;
using FluentAssertions;
using NUnit.Framework;

namespace AsyncRestClient.IntegrationTests
{
    public class When_posting_a_hello_request_and_a_long_running_request
    {
        readonly ConcurrentQueue<HelloResponse> helloResponses = new ConcurrentQueue<HelloResponse>();

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            CallApi("sleep");
            CallApi("world");
            Thread.Sleep(2000);
        }

        [Test]
        public void Then_first_response_should_be_name()
        {
            helloResponses.First().Reply.Should().EndWith("world");
        }

        [Test]
        public void Then_second_response_should_be_sleep()
        {
            helloResponses.ElementAt(1).Reply.Should().EndWith("sleep");
        }

        async void CallApi(string name)
        {
            var helloResponse = await SetUpFixture.AsyncClient.Post<Hello, HelloResponse>(new Hello {Name = name}, "hello");
            helloResponses.Enqueue(helloResponse);
        }
    }
}