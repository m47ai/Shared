namespace M47.Shared.Infrastructure.Integration.Bus.Event.AwsEventBridge;

using M47.Shared.Domain.Bus.Event;
using M47.Shared.Domain.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class AwsEventBridgeBus : IEventBus
{
    private readonly IAwsEventBridgePublisher _publisher;
    private readonly AwsEventBridgeConfiguration _configuration;
    private readonly IEnumerable<string> _validBusNames;

    public AwsEventBridgeBus(IAwsEventBridgePublisher publisher, IOptions<AwsEventBridgeConfiguration> configuration)
    {
        _publisher = publisher;
        _configuration = configuration.Value;
        _validBusNames = new List<string>() { "m47-dev", "m47-staging", "m47-prod" };
    }

    public async Task<IEnumerable<string>> PublishAsync(List<DomainEvent> events)
    {
        CheckBusname(_validBusNames, _configuration.BusName);

        var results = new List<string>();

        foreach (var @event in events)
        {
            var result = await PublishAsync(@event);

            if (!string.IsNullOrEmpty(result))
            {
                results.Add(result);
            }
        }

        return results;
    }

    private async Task<string> PublishAsync(DomainEvent domainEvent)
    {
        var message = new Message<DomainEvent>(domainEvent);

        return await _publisher.SendAsync(_configuration.BusName!, message);
    }

    private static void CheckBusname(IEnumerable<string> validBusNames, string? busName)
    {
        if (!validBusNames.Contains(busName!))
        {
            throw new AwsEventBridgeBusNameInvalidException(busName);
        }
    }
}