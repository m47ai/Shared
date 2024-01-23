namespace M47.Shared.Infrastructure.Storage.Aws;

using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using M47.Shared.Infrastructure.Compress;
using M47.Shared.Infrastructure.Storage.Model;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

public class AwsS3Storage : IStorage
{
    private readonly IAmazonS3 _client;
    private readonly ICompressor _compressor;

    public AwsS3Storage(IAmazonS3 client, ICompressor compressor)
    {
        _client = client;
        _compressor = compressor;
    }

    public async Task<string> SaveAsync(RequestSaveStorage request, CancellationToken cancellation = default)
    {
        if (request.IsCompressed)
        {
            var compressedStream = await _compressor.CompressUtf8Async(request.ContentBody!, cancellation);
            request.SetInputStream(compressedStream);
        }

        if (request.HasStream())
        {
            return await SaveStreamAsync(request, cancellation);
        }

        return await SaveTextAsync(request, cancellation);
    }

    public async Task<string> SaveStreamAsync(RequestSaveStorage request, CancellationToken cancellation)
    {
        var response = await _client.PutObjectAsync(new PutObjectRequest
        {
            InputStream = request.InputStream,
            Key = request.KeyName,
            CannedACL = GetS3CannedAcl(request),
            BucketName = request.BucketName,
        }, cancellation);

        return response.ResponseMetadata.RequestId;
    }

    public async Task<T> GetAsync<T>(RequestGetStorage request, CancellationToken cancellation = default)
    {
        if (request.IsCompressed)
        {
            return await GetCompressedObjectAsync<T>(request, cancellation);
        }

        return await GetObjectAsync<T>(request, cancellation);
    }

    public async Task<string> GetAsync(RequestGetStorage request, CancellationToken cancellation = default)
    {
        if (request.IsCompressed)
        {
            return await GetCompressedObjectAsync(request, cancellation);
        }

        return await GetObjectAsync(request, cancellation);
    }

    public async Task<string> DeleteAsync(RequestDeleteStorage request, CancellationToken cancellation = default)
    {
        var response = await _client.DeleteObjectAsync(new DeleteObjectRequest
        {
            BucketName = request.BucketName,
            Key = request.KeyName
        }, cancellation);

        return response.ResponseMetadata.RequestId;
    }

    public string GeneratePreSignedUrl(string s3Uri, int? lifeTimeInSeconds = null, bool isDownloadDelivery = false)
    {
        var awsS3Uri = new AmazonS3Uri(s3Uri);
        var presignedRequest = RequestGeneratePreSignedUrl.CreateForGet(awsS3Uri.Bucket, awsS3Uri.Key, lifeTimeInSeconds,
                                                                        isDownloadDelivery);

        return GeneratePreSignedUrl(presignedRequest);
    }

    public string GeneratePreSignedUrl(RequestGeneratePreSignedUrl request)
    {
        if (string.IsNullOrWhiteSpace(request.BucketName))
        {
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
            throw new ArgumentNullException(nameof(request.BucketName));
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
        }

        if (string.IsNullOrWhiteSpace(request.KeyName))
        {
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
            throw new ArgumentNullException(nameof(request.KeyName));
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
        }

        var presignedUrlRequest = new GetPreSignedUrlRequest
        {
            Key = request.KeyName,
            BucketName = request.BucketName,
            Verb = request.Verb,
            Expires = DateTime.UtcNow.AddSeconds(request.LifeTimeInSeconds)
        };

        if (request.IsDownloadDelivery && request.Verb == HttpVerb.GET)
        {
            presignedUrlRequest.ResponseHeaderOverrides = new ResponseHeaderOverrides()
            {
                ContentDisposition = $"attachment;"
            };
        }

        var url = _client.GetPreSignedURL(presignedUrlRequest);

        if (_client.Config.RegionEndpoint is null)
        {
            return url.Replace("://localstack:", "://localhost.localstack.cloud:");
        }

        return url;
    }

    public string GetPublicObjectUrl(string bucketName, string objectKey)
    {
        if (_client.Config.RegionEndpoint is null)
        {
            var uri = new Uri(_client.Config.ServiceURL);

            return $"https://{bucketName}.s3.{uri.Host}:{uri.Port}/{objectKey}";
        }

        return $"https://{bucketName}.s3-{_client.Config.RegionEndpoint?.SystemName}.amazonaws.com/{objectKey}";
    }

    public string GetS3Url(string bucketName, string keyName)
        => $"s3://{bucketName}/{keyName}";

    public async Task FileUploadAsync(RequestSaveStorage request, CancellationToken cancellation = default)
    {
        var utility = new TransferUtility(_client);
        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = request.InputStream,
            Key = request.KeyName,
            CannedACL = GetS3CannedAcl(request),
            BucketName = request.BucketName
        };

        await utility.UploadAsync(uploadRequest, cancellation);
    }

    public async Task FileUploadAsync(IFormFile file, string bucketName, string keyName,
                                      CancellationToken cancellation = default)
    {
        using var stream = file.OpenReadStream();

        var uploadRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = keyName,
            ContentType = file.ContentType, // "application/octet-stream",
            InputStream = stream,
            AutoCloseStream = true,
            Metadata =
            {
                ["x-amz-meta-originalname"] = file.FileName,
                ["x-amz-meta-extension"] = Path.GetExtension(file.FileName),
            }
        };

        await _client.PutObjectAsync(uploadRequest, cancellation);
    }

    public async Task DownloadFileAsync(string bucketName, string keyName, string pathToDownload, CancellationToken cancellation = default)
    {
        await _client.DownloadToFilePathAsync(bucketName, keyName, pathToDownload, new Dictionary<string, object>(), cancellation);
    }

    public async Task<Stream> DownloadFileStreamAsync(string bucketName, string keyName, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _client.GetObjectStreamAsync(bucketName, keyName, new Dictionary<string, object>(), cancellationToken);
        }
        catch (AmazonS3Exception ex)
        {
            if (ex.Message.Contains("Http Status Code NotFound"))
            {
                throw new FileNotFoundException($"No file found for the given excel fileId - {keyName}, {ex.Message}");
            }

            throw;
        }
    }

    public async Task<Stream> OpenStreamFromS3UrlAsync(string s3Url, CancellationToken cancellationToken = default)
    {
        var s3uri = new AmazonS3Uri(s3Url);

        return await DownloadFileStreamAsync(s3uri.Bucket, s3uri.Key, cancellationToken);
    }

    public async Task<string> DownloadCompressedS3UrlAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        using var stream = await OpenStreamFromS3UrlAsync(fileUrl, cancellationToken);

        return await _compressor.DecompressAsync(stream, Encoding.UTF8, cancellationToken);
    }

    public async Task<string> GetObjectFromS3UrlAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        var s3uri = new AmazonS3Uri(fileUrl);
        var request = RequestGetStorage.Object(s3uri.Bucket, s3uri.Key);

        return await GetObjectAsync(request, cancellationToken);
    }

    private async Task<T> GetCompressedObjectAsync<T>(RequestGetStorage request, CancellationToken cancellation)
    {
        var response = await _client.GetObjectAsync(request.BucketName, request.KeyName, cancellation);

        var data = await _compressor.DecompressAsync(response.ResponseStream, request.Encoding, cancellation);

        return JsonConvert.DeserializeObject<T>(data!)!;
    }

    private async Task<string> GetCompressedObjectAsync(RequestGetStorage request, CancellationToken cancellation)
    {
        var response = await _client.GetObjectAsync(request.BucketName, request.KeyName, cancellation);

        var data = await _compressor.DecompressAsync(response.ResponseStream, request.Encoding, cancellation);

        return data!;
    }

    private async Task<T> GetObjectAsync<T>(RequestGetStorage request, CancellationToken cancellation)
    {
        var response = await _client.GetObjectAsync(request.BucketName, request.KeyName, cancellation);

        using var reader = new StreamReader(response.ResponseStream);
        var data = await reader.ReadToEndAsync(cancellation);

        return JsonConvert.DeserializeObject<T>(data)!;
    }

    private async Task<string> GetObjectAsync(RequestGetStorage request, CancellationToken cancellation)
    {
        var response = await _client.GetObjectAsync(request.BucketName, request.KeyName, cancellation);

        using var reader = new StreamReader(response.ResponseStream);

        return await reader.ReadToEndAsync(cancellation);
    }

    private async Task<string> SaveTextAsync(RequestSaveStorage request, CancellationToken cancellation)
    {
        var respnose = await _client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = request.BucketName,
            Key = request.KeyName,
            CannedACL = GetS3CannedAcl(request),
            ContentType = request.ContentType,
            ContentBody = request.ContentBody
        }, cancellation);

        return respnose.ResponseMetadata.RequestId;
    }

    private static S3CannedACL GetS3CannedAcl(RequestSaveStorage request)
    {
        if (request.StorageAccessMode == StorageAccesMode.PublicRead)
        {
            return S3CannedACL.PublicRead;
        }

        if (request.StorageAccessMode == StorageAccesMode.PublicReadWrite)
        {
            return S3CannedACL.PublicReadWrite;
        }

        return S3CannedACL.Private;
    }
}