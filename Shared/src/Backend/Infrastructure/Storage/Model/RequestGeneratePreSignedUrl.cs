namespace M47.Shared.Infrastructure.Storage.Model;

using Amazon.S3;

public sealed class RequestGeneratePreSignedUrl
{
    private const int _maxPresignedUrlLifetimeInSeconds = 7 * 24 * 60 * 60;

    public static RequestGeneratePreSignedUrl CreateWithMaxLifetimeForGet(string bucketName, string key, bool isDownloadDelivery = false)
        => new(bucketName, key, HttpVerb.GET, _maxPresignedUrlLifetimeInSeconds, isDownloadDelivery);

    public static RequestGeneratePreSignedUrl CreateWithMaxLifetimeForUpdate(string bucketName, string key, bool isDownloadDelivery = false)
    => new(bucketName, key, HttpVerb.PUT, _maxPresignedUrlLifetimeInSeconds, isDownloadDelivery);

    public static RequestGeneratePreSignedUrl CreateForGet(string bucketName, string key, int? lifeTimeInSeconds = null, bool isDownloadDelivery = false)
        => new(bucketName, key, HttpVerb.GET, lifeTimeInSeconds ?? _maxPresignedUrlLifetimeInSeconds, isDownloadDelivery);

    public readonly string BucketName;
    public readonly string KeyName;
    public readonly HttpVerb Verb;
    public readonly int LifeTimeInSeconds;
    public readonly bool IsDownloadDelivery;

    private RequestGeneratePreSignedUrl(string bucketName, string keyName, HttpVerb httpVerb, int lifeTimeInSeconds, bool isDownloadDelivery = false)
    {
        BucketName = bucketName;
        KeyName = keyName;
        Verb = httpVerb;
        LifeTimeInSeconds = lifeTimeInSeconds;
        IsDownloadDelivery = isDownloadDelivery;
    }
}