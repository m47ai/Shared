namespace M47.Shared.Infrastructure.Storage.Model;

public sealed class RequestDeleteStorage
{
    public static RequestDeleteStorage Object(string bucketName, string keyName)
        => new(bucketName, keyName);

    public readonly string BucketName;
    public readonly string KeyName;

    private RequestDeleteStorage(string bucketName, string keyName)
    {
        BucketName = bucketName;
        KeyName = keyName;
    }
}