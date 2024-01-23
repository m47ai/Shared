namespace M47.Shared.Infrastructure.Storage.Model;

using System.Text;

public sealed class RequestGetStorage
{
    public static RequestGetStorage Descompress(string bucketName, string keyName)
        => new(bucketName, keyName, Encoding.UTF8, isGzip: true);

    public static RequestGetStorage Object(string bucketName, string keyName)
        => new(bucketName, keyName, Encoding.UTF8, isGzip: false);

    public readonly string BucketName;
    public readonly string KeyName;
    public readonly bool IsCompressed;
    public readonly Encoding Encoding;

    private RequestGetStorage(string bucketName, string keyName, Encoding encoding, bool isGzip)
    {
        BucketName = bucketName;
        KeyName = keyName;
        IsCompressed = isGzip;
        Encoding = encoding;
    }
}