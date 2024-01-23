namespace M47.Shared.Infrastructure.Storage;

using M47.Shared.Infrastructure.Storage.Model;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public interface IStorage
{
    Task<string> SaveAsync(RequestSaveStorage request, CancellationToken cancellation = default);

    Task<string> SaveStreamAsync(RequestSaveStorage request, CancellationToken cancellation = default);

    Task<T> GetAsync<T>(RequestGetStorage request, CancellationToken cancellation = default);

    Task<string> GetAsync(RequestGetStorage request, CancellationToken cancellation = default);

    Task<string> DeleteAsync(RequestDeleteStorage request, CancellationToken cancellation = default);

    string GeneratePreSignedUrl(string s3Uri, int? lifeTimeInSeconds = null, bool isDownloadDelivery = false);

    string GeneratePreSignedUrl(RequestGeneratePreSignedUrl request);

    string GetPublicObjectUrl(string bucketName, string keyName);

    string GetS3Url(string bucketName, string keyName);

    Task FileUploadAsync(RequestSaveStorage request, CancellationToken cancellation = default);

    Task FileUploadAsync(IFormFile file, string bucketName, string keyName, CancellationToken cancellation = default);

    [Obsolete("Pending to refactor, a request object must be used instead")]
    Task DownloadFileAsync(string bucketName, string keyName, string pathToDownload, CancellationToken cancellation = default);

    [Obsolete("Pending to refactor, a request object must be used instead")]
    Task<Stream> DownloadFileStreamAsync(string bucketName, string keyName, CancellationToken cancellation = default);

    Task<Stream> OpenStreamFromS3UrlAsync(string s3Url, CancellationToken cancellation = default);

    Task<string> DownloadCompressedS3UrlAsync(string s3Url, CancellationToken cancellationToken = default);

    Task<string> GetObjectFromS3UrlAsync(string s3Url, CancellationToken cancellationToken = default);
}