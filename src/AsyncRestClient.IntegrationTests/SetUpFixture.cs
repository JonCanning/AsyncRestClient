using System;
using Nancy.Hosting.Self;
using NUnit.Framework;

namespace AsyncRestClient.IntegrationTests
{
    [SetUpFixture]
    class SetUpFixture
    {
        public static IAsyncClient AsyncClient;
        NancyHost host;

        [SetUp]
        public void TestFixtureSetUp()
        {
            var hostConfiguration = new HostConfiguration {UrlReservations = new UrlReservations {CreateAutomatically = true}};
            var baseAddress = new Uri("http://127.0.0.1:1337/");
            host = new NancyHost(hostConfiguration, baseAddress);
            host.Start();
            AsyncClient = new AsyncClient(baseAddress);
        }

        [TearDown]
        public void TestFixtureTearDown()
        {
            AsyncClient.Dispose();
            host.Stop();
            host.Dispose();
        }
    }
}