namespace M47.Shared.Tests.Infrastructure.Storage.Aws;

using M47.Shared.Infrastructure.Compress;
using M47.Shared.Infrastructure.Storage.Aws;
using M47.Shared.Infrastructure.Storage.Model;
using M47.Shared.Tests.Factory;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

[Collection(nameof(SharedFactoryShared))]
public class AwsS3StorageTests
{
    private readonly string _bucket = "mybucket";
    private readonly AwsS3Storage _storage;
    private readonly string _baseUrl;

    public AwsS3StorageTests(SharedFactory factory)
    {
        var client = factory.Localstack.AwsHelper.CreateS3Client();

        _baseUrl = client.Config.ServiceURL;
        _storage = new AwsS3Storage(client, new GzipCompressor());
    }

    [Fact]
    public async Task Should_Save_When_JsonText()
    {
        // Arrange
        const string expected = @"{""color"": ""Red🤙 💪 🦵 🦶 🖕 ✍️ 🙏 💍 💄 💋 👄 👅 👂 👃 👣 👁 👀 🧠 🦴 🦷 🗣 👤 👥""}";
        var keyname = Guid.NewGuid().ToString();
        var requestSave = RequestSaveStorage.PrivateCompressObject(_bucket, keyname, expected);

        // Act
        await _storage.SaveAsync(requestSave);

        // Assert
        var actual = await _storage.GetAsync<string>(RequestGetStorage.Descompress(_bucket, keyname));
        await Verify(actual).UseDirectory("Sample/Verified");
    }

    [Fact]
    public async Task Should_ReadPublicObject_When_GetUrl()
    {
        // Arrange
        const string expected = "hi world!";
        var keyname = Guid.NewGuid().ToString();
        var requestPublicSave = new RequestSaveStorage(_bucket, keyname, StorageAccesMode.PublicRead,
                                                       isCompressed: false, contentBody: expected);
        await _storage.SaveAsync(requestPublicSave);

        // Act
        var objectUrl = $"{_baseUrl}{_bucket}/{keyname}";
        var actual = await GetContentAsync(objectUrl);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public async Task Should_ReadObject_When_IsCreatePresignedUrl()
    {
        // Arrange
        var fileName = Guid.NewGuid().ToString();
        const string expectedContent = "hi world!";

        // Act
        var requestSave = new RequestSaveStorage(_bucket, fileName, StorageAccesMode.Private,
                                                 isCompressed: false, contentBody: expectedContent);
        await _storage.SaveAsync(requestSave);
        var requestPresigned = RequestGeneratePreSignedUrl.CreateWithMaxLifetimeForGet(_bucket, fileName);
        var presignedUrl = _storage.GeneratePreSignedUrl(requestPresigned);
        var actualContent = await GetContentAsync(presignedUrl);
        var requestDelete = RequestDeleteStorage.Object(_bucket, fileName);
        await _storage.DeleteAsync(requestDelete);

        // Assert
        actualContent.Should().Be(expectedContent);
    }

    [Fact]
    public async Task Should_UploadFile_When_PutPresignedUrlIsRequested()
    {
        // Arrange
        var fileName = @"/tmp/TestPresignedFile.txt";
        var expectedMessage = "Author:PresignedTester";
        await File.WriteAllTextAsync(fileName, expectedMessage);
        var requestPutPresigned = RequestGeneratePreSignedUrl.CreateWithMaxLifetimeForUpdate(_bucket, fileName);

        // Act
        var preSignedUrl = _storage.GeneratePreSignedUrl(requestPutPresigned);

        using var file = File.OpenRead(fileName);
        await PutContentAsync(preSignedUrl, file);

        // Assert
        var actualContent = await GetContentAsync(preSignedUrl);
        actualContent.Trim().Should().Be(expectedMessage);

        var requestDelete = RequestDeleteStorage.Object(_bucket, fileName);
        await _storage.DeleteAsync(requestDelete);
    }

    [Fact]
    public async Task Should_UploadFile_When_Requested()
    {
        // Arrange
        var fileName = @"/tmp/TestFile.txt";
        var expectedMessage = "Author:Tester";
        await File.WriteAllTextAsync(fileName, expectedMessage);
        var fileStream = File.OpenRead(fileName);
        var request = new RequestSaveStorage(_bucket, fileName, StorageAccesMode.Private, isCompressed: false, inputStream: fileStream);

        // Act
        await _storage.FileUploadAsync(request);

        var requestPresigned = RequestGeneratePreSignedUrl.CreateWithMaxLifetimeForGet(_bucket, fileName);
        var presignedUrl = _storage.GeneratePreSignedUrl(requestPresigned);
        var actualContent = await GetContentAsync(presignedUrl);
        var requestDelete = RequestDeleteStorage.Object(_bucket, fileName);
        await _storage.DeleteAsync(requestDelete);

        // Assert
        actualContent.Trim().Should().Be(expectedMessage);
    }

    [Fact]
    public async Task Should_DownloadFile_When_Requested()
    {
        // Arrange
        var fileName = "TestFile.txt";
        var expectedMessage = "Author:Tester";
        var downloadPath = $"/tmp/{Guid.NewGuid()}.txt";

        await File.WriteAllTextAsync(fileName, expectedMessage);
        var fileStream = File.OpenRead(fileName);
        var request = new RequestSaveStorage(_bucket, fileName, StorageAccesMode.Private, isCompressed: false, inputStream: fileStream);
        await _storage.FileUploadAsync(request);

        // Act
        await _storage.DownloadFileAsync(_bucket, fileName, downloadPath);

        var requestDelete = RequestDeleteStorage.Object(_bucket, fileName);
        await _storage.DeleteAsync(requestDelete);
        var isFileDownload = File.Exists(downloadPath);
        var actualMessage = File.ReadAllText(downloadPath);
        File.Delete(downloadPath);

        // Assert
        actualMessage.Trim().Should().Be(expectedMessage);
        isFileDownload.Should().BeTrue();
    }

    [Fact]
    public async Task Should_DownloadFileStream_When_Requested()
    {
        // Arrange
        var fileName = @"/tmp/TestFile.txt";
        var expectedMessage = "Author:Tester";

        await File.WriteAllTextAsync(fileName, expectedMessage);
        var fileStream = File.OpenRead(fileName);
        var request = new RequestSaveStorage(_bucket, fileName, StorageAccesMode.Private, isCompressed: false, inputStream: fileStream);
        await _storage.FileUploadAsync(request);

        // Act
        var actualStream = await _storage.DownloadFileStreamAsync(_bucket, fileName);

        var requestDelete = RequestDeleteStorage.Object(_bucket, fileName);
        await _storage.DeleteAsync(requestDelete);

        // Assert
        actualStream.Should().NotBeNull();
    }

    private static async Task<string> PutContentAsync(string preSignedUrl, Stream stream)
    {
        using var client = CreateHttpClient(preSignedUrl);
        using var streamContent = new StreamContent(stream);

        var response = await client.PutAsync(preSignedUrl, streamContent);

        return await GetResponseAsync(response);
    }

    private static async Task<string> GetContentAsync(string preSignedUrl)
    {
        using var client = CreateHttpClient(preSignedUrl);

        var response = await client.GetAsync(preSignedUrl);

        return await GetResponseAsync(response);
    }

    private static HttpClient CreateHttpClient(string preSignedUrl)
    {
        var httpClientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
        };

        return new HttpClient(httpClientHandler) { BaseAddress = new Uri(preSignedUrl) };
    }

    private static async Task<string> GetResponseAsync(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        var content = response.Content.ReadAsStream();
        var reader = new StreamReader(content);

        return await reader.ReadToEndAsync();
    }
}