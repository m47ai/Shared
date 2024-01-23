namespace M47.Shared.Domain.Api;

public class DataWrapper<T, K> : DataWrapper<T>
{
    public K Include { get; set; }

    public DataWrapper(T data, K include, IDictionary<string, object>? metadata = null) : base(data, metadata)
    {
        Include = include;
    }
}

public class DataWrapper<T>
{
    public T Data { get; set; }
    public IDictionary<string, object>? Metadata { get; set; }

    public DataWrapper(T data, IDictionary<string, object>? metadata = null)
    {
        Data = data;
        Metadata = metadata;
    }
}