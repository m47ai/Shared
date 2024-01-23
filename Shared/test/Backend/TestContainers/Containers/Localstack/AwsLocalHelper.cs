namespace M47.Shared.Tests.TestContainers.Containers.Localstack;

using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Util;
using Amazon.SimpleEmail;
using Amazon.SQS;
using Amazon.SQS.Model;
using M47.Shared.ConfigurationServices;
using M47.Shared.Infrastructure.Compress;
using M47.Shared.Infrastructure.Storage.Aws;
using M47.Shared.Infrastructure.Storage.Model;
using M47.Shared.Tests.Utils.Sample;
using System.Net;
using System.Threading.Tasks;

public static class AwsFakeCredentials
{
    public const string AwsAccessKey = "AKIAIOSFODNN7EXAMPLE";

    public const string AwsSecretKey = "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY";
}

public class AwsLocalHelper
{
    private readonly AWSCredentials _credentials;
    private readonly string _serviceUrl;

    public AwsLocalHelper(string serviceUrl)
    {
        _credentials = new BasicAWSCredentials(AwsFakeCredentials.AwsAccessKey, AwsFakeCredentials.AwsSecretKey);
        _serviceUrl = serviceUrl;
    }

    public AwsLocalHelper(AWSCredentials credentials, string serviceUrl)
    {
        _credentials = credentials;
        _serviceUrl = serviceUrl;
    }

    public IAmazonS3 CreateS3Client()
        => AwsLocalConfiguration.CreateS3Client(_credentials, _serviceUrl);

    public IAmazonSQS CreateSqsClient()
        => AwsLocalConfiguration.CreateSqsClient(_credentials, _serviceUrl);

    public IAmazonSimpleEmailService CreateSesClient()
        => AwsLocalConfiguration.CreateSesClient(_credentials, _serviceUrl);

    public async Task<string> UploadFileToBucketAndGetPresignedUrlAsync<T>(string filename, string bucketName = "mybucket")
    {
        var storage = new AwsS3Storage(CreateS3Client(), new GzipCompressor());
        using var stream = SampleFile.OpenAsStream<T>(filename);

        await UploadFileToBucketAndKeyAsync<T>(filename, filename, bucketName);

        var presigned = RequestGeneratePreSignedUrl.CreateWithMaxLifetimeForGet(bucketName, filename);

        return storage.GeneratePreSignedUrl(presigned);
    }

    public async Task<string> UploadFileToBucketAndGetS3UrlAsync<T>(string filename, string bucketName = "mybucket")
    {
        var storage = new AwsS3Storage(CreateS3Client(), new GzipCompressor());

        await UploadFileToBucketAndKeyAsync<T>(filename, filename, bucketName);

        return storage.GetS3Url(bucketName, filename);
    }

    public async Task UploadFileToBucketAndKeyAsync<T>(string filename, string keyName, string bucketName = "mybucket")
    {
        var storage = new AwsS3Storage(CreateS3Client(), new GzipCompressor());

        using var stream = SampleFile.OpenAsStream<T>(filename);

        var request = RequestSaveStorage.PublicStream(bucketName, keyName, stream);

        await storage.FileUploadAsync(request);
    }

    public async Task<string> SaveObjectAndGetS3UrlAsync(string contentBody, string keyName, string bucketName = "mybucket")
    {
        var storage = new AwsS3Storage(CreateS3Client(), new GzipCompressor());

        var request = RequestSaveStorage.PublicJson(bucketName, keyName, contentBody);

        await storage.SaveAsync(request);

        return storage.GetS3Url(bucketName, keyName);
    }

    public async Task<string> SaveCompressAndGetS3UrlAsync(string contentBody, string keyName, string bucketName = "mybucket")
    {
        var storage = new AwsS3Storage(CreateS3Client(), new GzipCompressor());

        var request = RequestSaveStorage.PublicCompressJson(bucketName, keyName, contentBody);

        await storage.SaveAsync(request);

        return storage.GetS3Url(bucketName, keyName);
    }

    public async Task<string> DownloadCompressedS3UrlAsync(string s3Url)
    {
        var storage = new AwsS3Storage(CreateS3Client(), new GzipCompressor());

        return await storage.DownloadCompressedS3UrlAsync(s3Url);
    }

    public async Task<string> DownloadS3UrlAsync(string s3Url)
    {
        var storage = new AwsS3Storage(CreateS3Client(), new GzipCompressor());
        var s3Uri = new AmazonS3Uri(s3Url);

        var request = RequestGetStorage.Object(s3Uri.Bucket, s3Uri.Key);

        return await storage.GetAsync(request);
    }

    public async Task<int> GetApproximateNumberOfMessagesAsync(string queueId)
    {
        var sqsClient = CreateSqsClient();

        var response = await sqsClient.GetQueueUrlAsync(queueId);

        var request = new GetQueueAttributesRequest
        {
            QueueUrl = response.QueueUrl,
            AttributeNames = new List<string> { "ApproximateNumberOfMessages" }
        };

        var attributesResponse = await sqsClient.GetQueueAttributesAsync(request);

        return attributesResponse.ApproximateNumberOfMessages;
    }

    public async Task<HttpStatusCode> PurgeQueueAsync(string queueId)
    {
        var sqsClient = CreateSqsClient();

        var response = await sqsClient.GetQueueUrlAsync(queueId);

        var request = new PurgeQueueRequest
        {
            QueueUrl = response.QueueUrl,
        };

        var purgeResponse = await sqsClient.PurgeQueueAsync(request);

        return purgeResponse.HttpStatusCode;
    }
}