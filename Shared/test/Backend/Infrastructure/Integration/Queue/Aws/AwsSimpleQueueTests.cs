namespace M47.Shared.Tests.Infrastructure.Integration.Queue.Aws;

using Amazon.SQS;
using M47.Shared.Infrastructure.Integration.Queue;
using M47.Shared.Infrastructure.Integration.Queue.Aws;
using M47.Shared.Tests.Factory;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;

[Collection(nameof(SharedFactoryShared))]
public sealed class AwsSimpleQueueTests
{
    private readonly IQueue _queue;
    private readonly IAmazonSQS _sqsClient;
    private readonly string queueUrl = "simple-queue-test";

    public AwsSimpleQueueTests(SharedFactory factory)
    {
        _sqsClient = factory.Localstack.AwsHelper.CreateSqsClient();
        _queue = new AwsSimpleQueue(_sqsClient);
    }

    [Fact]
    public async Task Should_SendReadAndDeleteMessages_When_AreValids()
    {
        // Arrange
        var message = "Hi world!";

        // Act
        var sendResponse = await _queue.SendMessageAsync(queueUrl, message);
        var readResponse = await _queue.PullMessagesAsync(queueUrl, nMessagesToRetrieve: 1);
        var deleteResponse = await _queue.DeleteMessageAsync(queueUrl, readResponse.Keys.First());

        // Assert
        await Verify(new
        {
            sendResponse,
            readResponse.First().Value,
            deleteResponse = RemoveJsonKey(deleteResponse, "ResponseMetadata.RequestId", "ContentLength")
        }).UseDirectory("Sample/Verified");
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
}