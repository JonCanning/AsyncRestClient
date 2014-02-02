using System;
using System.IO;
using System.Linq;
using System.Threading;
using Nancy;
using Nancy.ModelBinding;

namespace AsyncRestClient.IntegrationTests.Service
{
    public class HelloModule : NancyModule
    {
        public HelloModule()
        {
            Post["/hello"] = o =>
            {
                var hello = this.Bind<Hello>();
                if (Request.Files.Any())
                {
                    var file = Request.Files.First();
                    var fileStream = File.OpenWrite(Path.Combine(Environment.CurrentDirectory, file.Name));
                    using (var streamWriter = new StreamWriter(fileStream))
                        streamWriter.Write(new StreamReader(file.Value).ReadToEnd());
                }
                if (hello.Name == "sleep")
                    Thread.Sleep(1000);
                return new HelloResponse {Reply = "Hello, " + hello.Name};
            };

            Get["/hello/{Name}"] = o => new HelloResponse {Reply = "Hello, " + o.Name};
        }
    }
}