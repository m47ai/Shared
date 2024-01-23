namespace M47.Shared.Tests.Infrastructure.Integration.Queue.Aws;

using Amazon.SQS;
using M47.Shared.Infrastructure.Integration.Queue;
using M47.Shared.Infrastructure.Integration.Queue.Aws;
using M47.Shared.Tests.Factory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;

[Collection(nameof(SharedFactoryShared))]
public sealed class AwsSimpleTypedQueueTests
{
    private readonly IQueue<WebsiteRank> _queueWisetRankTyped;
    private readonly IQueue _queue;
    private readonly IAmazonSQS _sqsClient;
    private readonly string _queueUrl = "typed-queue-test";

    public AwsSimpleTypedQueueTests(SharedFactory factory)
    {
        _sqsClient = factory.Localstack.AwsHelper.CreateSqsClient();
        _queue = new AwsSimpleQueue(_sqsClient);
        _queueWisetRankTyped = new AwsSimpleQueue<WebsiteRank>(_queue);
    }

    [Fact]
    public async Task Should_SendReadAndDeleteTypedMessages_When_AreValids()
    {
        // Arrange
        var message = new WebsiteRank
        {
            ContentsCount = 10,
            DomainName = "m47.com",
            Rank = 3,
        };

        // Act
        var sendResponse = await _queueWisetRankTyped.SendMessageAsync(_queueUrl, message);
        var readResponse = await _queueWisetRankTyped.PullMessagesAsync(_queueUrl, nMessagesToRetrieve: 1);
        var deleteResponse = await _queueWisetRankTyped.DeleteMessageAsync(_queueUrl, readResponse.Keys.First());

        // Assert
        await VerifyResponse(sendResponse, readResponse, deleteResponse);
    }

    [Fact]
    public async Task Should_ReturnCamelCaseJsonResponse_When_MessageCameFromSerializedQueue()
    {
        // Arrange
        var message = new WebsiteRank
        {
            ContentsCount = 10,
            DomainName = "m47.com",
            Rank = 3,
        };

        // Act
        var sendResponse = await _queueWisetRankTyped.SendMessageAsync(_queueUrl, message);
        var readResponse = await _queue.PullMessagesAsync(_queueUrl, nMessagesToRetrieve: 1);
        var deleteResponse = await _queueWisetRankTyped.DeleteMessageAsync(_queueUrl, readResponse.Keys.First());

        // Assert
        await VerifyResponse(sendResponse, readResponse, deleteResponse);
    }

    [Fact]
    public async Task Should_ReturnDeserializedObject_When_MessageCameFromSerializedQueue()
    {
        // Arrange
        const string expected = @"{
                                      'rank': 79,
                                      'domainName': 'DomainName354da89b-89f9-4f01-9b81-454bd85c3790',
                                      'contentsCount': 159
                                     }";

        // Act
        var sendResponse = await _queue.SendMessageAsync(_queueUrl, expected);
        var readResponse = await _queueWisetRankTyped.PullMessagesAsync(_queueUrl, nMessagesToRetrieve: 1);
        var deleteResponse = await _queueWisetRankTyped.DeleteMessageAsync(_queueUrl, readResponse.Keys.First());

        // Assert
        await VerifyResponse(sendResponse, readResponse, deleteResponse);
    }

    [Fact]
    public async Task Should_ReturnDesiarialzedObjectPascaCase_When_MessageCameFromSerializedQueue()
    {
        // Arrange
        const string expected = @"{
                                      'Rank': 79,
                                      'DomainName': 'DomainName354da89b-89f9-4f01-9b81-454bd85c3790',
                                      'ContentsCount': 159
                                     }";

        // Act
        var sendResponse = await _queue.SendMessageAsync(_queueUrl, expected);
        var readResponse = await _queueWisetRankTyped.PullMessagesAsync(_queueUrl, nMessagesToRetrieve: 1);
        var deleteResponse = await _queueWisetRankTyped.DeleteMessageAsync(_queueUrl, readResponse.Keys.First());

        // Assert
        await VerifyResponse(sendResponse, readResponse, deleteResponse);
    }

    [Fact]
    public async Task Should_ThrowJsonReaderException_When_MessageIsJsonInvalid()
    {
        // Arrange
        const string expected = "string";
        _ = await _queue.SendMessageAsync(_queueUrl, expected);

        // Act
        var action = () => _queueWisetRankTyped.PullMessagesAsync(_queueUrl, nMessagesToRetrieve: 1);

        // Assert
        await action.Should().ThrowAsync<JsonReaderException>();
    }

    private class WebsiteRank
    {
        public int Rank { get; set; }
        public string? DomainName { get; set; }
        public int ContentsCount { get; set; }
    }

    private static string RemoveJsonKey(string deleteResponse, params string[] keys)
    {
        var json = JObject.Parse(deleteResponse);
        foreach (var key in keys)
        {
            var splitted = key.Split(".");
            if (splitted.Length == 2)
            {
                var header = (JObject)json.SelectToken(splitted[0])!;
                header.Property(splitted[1])!.Remove();
            }
            else
            {
                json.Remove(key);
            }
        }

        return json.ToString();
    }

    private static async Task VerifyResponse<T>(string sendResponse, Dictionary<string, T> readResponse, string deleteResponse)
    {
        var settings = new VerifySettings();
        settings.ScrubMember("Content-Length");
        await Verify(new
        {
            sendResponse,
            readResponse.First().Value,
            deleteResponse = RemoveJsonKey(deleteResponse, "ResponseMetadata.RequestId", "ContentLength")
        }, settings)
         .UseDirectory("Sample/Verified");
    }
}