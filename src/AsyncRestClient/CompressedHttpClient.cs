using System.Net;
using System.Net.Http;

namespace AsyncRestClient
{
    public class CompressedHttpClient : HttpClient
    {
        public CompressedHttpClient()
            : base(new HttpClientHandler {AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip}) {}
    }
}