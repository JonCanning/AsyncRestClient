using Newtonsoft.Json;

namespace AsyncRestClient
{
    public class JsonNetSerializer : ISerializer
    {
        public JsonNetSerializer()
        {
            ContentType = "application/json";
        }

        public string ContentType { get; private set; }

        public string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}