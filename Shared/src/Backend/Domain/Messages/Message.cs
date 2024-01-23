namespace M47.Shared.Domain.Messages;

using M47.Shared.Domain.Bus.Event;

public class Message<T> where T : IBaseEvent
{
    public T Data { get; set; }
    public MessageMeta? Meta { get; set; }

    public Message(DomainEvent domainEvent)
    {
        Data = (T)(domainEvent as object);
        Meta = MessageMeta.Create(domainEvent);
    }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

    public Message()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    {
    }
}