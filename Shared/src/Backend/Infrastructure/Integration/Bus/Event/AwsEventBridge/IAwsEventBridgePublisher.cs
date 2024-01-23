namespace M47.Shared.Infrastructure.Integration.Bus.Event.AwsEventBridge;

using M47.Shared.Domain.Bus.Event;
using M47.Shared.Domain.Messages;
using System.Threading.Tasks;

public interface IAwsEventBridgePublisher
{
    Task<string> SendAsync(string exchangeName, Message<DomainEvent> message);
}