namespace AsyncRestClient
{
    public interface ISerializer
    {
        string ContentType { get; }
        string Serialize<T>(T value);
        T Deserialize<T>(string value);
    }
}