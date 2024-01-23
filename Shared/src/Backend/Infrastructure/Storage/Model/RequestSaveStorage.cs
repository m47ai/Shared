namespace M47.Shared.Infrastructure.Storage.Model;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;

public class RequestSaveStorage
{
    public static RequestSaveStorage PrivateJson(string bucketName, string keyName, string contentBody)
        => new(bucketName, keyName, StorageAccesMode.Private, isCompressed: false, contentBody,
               contentType: "application/json");

    public static RequestSaveStorage PublicJson(string bucketName, string keyName, string contentBody)
        => new(bucketName, keyName, StorageAccesMode.PublicRead, isCompressed: false, contentBody,
               contentType: "application/json");

    public static RequestSaveStorage PrivateCompressString(string bucketName, string keyName, string contentBody)
        => new(bucketName, keyName, StorageAccesMode.Private, isCompressed: true, contentBody);

    public static RequestSaveStorage PublicCompressString(string bucketName, string keyName, string contentBody)
        => new(bucketName, keyName, StorageAccesMode.PublicRead, isCompressed: true, contentBody);

    public static RequestSaveStorage PublicCompressJson(string bucketName, string keyName, string contentBody)
        => new(bucketName, keyName, StorageAccesMode.PublicRead, isCompressed: true, contentBody,
               contentType: "application/json");

    public static RequestSaveStorage PrivateStream(string bucketName, string keyName, Stream inputStream)
        => new(bucketName, keyName, StorageAccesMode.Private, isCompressed: false, inputStream: inputStream);

    public static RequestSaveStorage PublicStream(string bucketName, string keyName, Stream inputStream)
        => new(bucketName, keyName, StorageAccesMode.Private, isCompressed: false, inputStream: inputStream);

    public static RequestSaveStorage PrivateCompressObject(string bucketName, string keyName, object @object)
    {
        var contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };

        var contentBody = JsonConvert.SerializeObject(@object, new JsonSerializerSettings
        {
            ContractResolver = contractResolver,
            Formatting = Formatting.Indented
        });

        return new RequestSaveStorage(bucketName, keyName, StorageAccesMode.Private, isCompressed: true,
                                      contentBody: contentBody);
    }

    public readonly string BucketName;
    public readonly string KeyName;
    public readonly StorageAccesMode StorageAccessMode;
    public readonly string? ContentBody;
    public readonly string? ContentType;
    public readonly bool IsCompressed;
    public Stream? InputStream { get; private set; }

    public RequestSaveStorage(string bucketName, string keyName, StorageAccesMode storageAccessMode,
                              bool isCompressed, string? contentBody = null, string? contentType = null,
                              Stream? inputStream = null)
    {
        BucketName = bucketName;
        KeyName = keyName;
        StorageAccessMode = storageAccessMode;
        ContentBody = contentBody;
        ContentType = contentType;
        IsCompressed = isCompressed;
        InputStream = inputStream;
    }

    public void SetInputStream(Stream stream)
    {
        if (InputStream is null && IsCompressed)
        {
            InputStream = stream;
        }
    }

    public bool HasStream() => InputStream is not null;
}