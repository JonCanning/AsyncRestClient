using System;
using System.IO;
using System.Text;
using AsyncRestClient.IntegrationTests.Service;
using FluentAssertions;
using NUnit.Framework;

namespace AsyncRestClient.IntegrationTests
{
    [TestFixture]
    class When_posting_a_hello_request_with_a_file
    {
        const string Filename = "fileName.txt";
        readonly string filePath = Path.Combine(Environment.CurrentDirectory, Filename);

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        [Test]
        public void Then_response_should_be_hello_name_and_file_should_be_saved()
        {
            const string filecontent = "fileContent";
            var response =
                SetUpFixture.AsyncClient.Post<Hello, HelloResponse>(
                    new Hello {Name = "world"},
                    "hello",
                    Filename,
                    new MemoryStream(Encoding.UTF8.GetBytes(filecontent))).Result;
            response.Reply.Should().Be("Hello, world");
            File.Exists(filePath).Should().BeTrue();
            File.ReadAllText(filePath).Should().Be(filecontent);
        }
    }
}