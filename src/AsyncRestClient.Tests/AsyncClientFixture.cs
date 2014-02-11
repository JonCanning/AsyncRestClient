namespace AsyncRestClient.Tests
{
    abstract class AsyncClientFixture
    {
        protected readonly AsyncClient AsyncClient;
        protected readonly StubHttpClient HttpClient;
        protected readonly ISerializer JsonNetSerializer;
        protected string RequestContent;

        protected AsyncClientFixture()
        {
            HttpClient = new StubHttpClient();
            JsonNetSerializer = new JsonNetSerializer();
            AsyncClient = new AsyncClient(HttpClient, JsonNetSerializer)
            {
                OnBeforeSend = request =>
                {
                    if (request.Content != null)
                        RequestContent = request.Content.ReadAsStringAsync().Result;
                }
            };
        }
    }
}