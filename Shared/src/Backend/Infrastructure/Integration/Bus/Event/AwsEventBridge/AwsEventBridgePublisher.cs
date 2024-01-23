namespace M47.Shared.Infrastructure.Integration.Bus.Event.AwsEventBridge;

using Amazon.EventBridge;
using Amazon.EventBridge.Model;
using M47.Shared.Domain.Bus.Event;
using M47.Shared.Domain.Messages;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

public class AwsEventBridgePublisher : IAwsEventBridgePublisher
{
    private readonly IAmazonEventBridge _client;
    private readonly ILogger<AwsEventBridgePublisher> _logger;
    private readonly DomainEventJsonSerializer _serializer;

    public AwsEventBridgePublisher(IAmazonEventBridge client, ILogger<AwsEventBridgePublisher> logger,
                                   DomainEventJsonSerializer serializer)
    {
        _client = client;
        _logger = logger;
        _serializer = serializer;
    }

    public async Task<string> SendAsync(string exchangeName, Message<DomainEvent> message)
    {
        var jsonMessage = DomainEventJsonSerializer.Serialize(message);

        var putEventsEntry = new PutEventsRequestEntry
        {
            EventBusName = exchangeName,
            Source = $"{message.Meta!.Topic!.Organization}.{message.Meta.Topic.Department}.{message.Meta.Topic.Service}",
            DetailType = message.Meta.Type,
            Detail = jsonMessage,
        };

        var putEventsRequest = new PutEventsRequest();
        putEventsRequest.Entries.Add(putEventsEntry);

        var response = await _client.PutEventsAsync(putEventsRequest);

        ManageError(response);

        return (response.Entries.FirstOrDefault()!.EventId) ?? string.Empty;
    }

    private void ManageError(PutEventsResponse response)
    {
        if (response.FailedEntryCount > 0)
        {
            _logger.LogWarning("Message from event bus failed: {responseEntries}", response.Entries);
        }
    }
}