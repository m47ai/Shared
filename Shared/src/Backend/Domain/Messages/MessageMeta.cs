namespace M47.Shared.Domain.Messages;

using M47.Shared.Domain.Bus.Event;
using System;

public class MessageMeta
{
    public Guid MessageId { get; set; }
    public string Type { get; set; }
    public MessageTopic? Topic { get; set; }
    public DateTime OccurredOn { get; set; }

    public static MessageMeta Create(DomainEvent domainEvent)
    {
        return new MessageMeta(domainEvent);
    }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

    private MessageMeta()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    {
    }

    private MessageMeta(DomainEvent domainEvent)
    {
        MessageId = domainEvent.GetEventId();
        Type = domainEvent.GetType().Name;
        Topic = new MessageTopic(domainEvent.GetFullQualifiedEventName());
        OccurredOn = domainEvent.GetOccurredOn();
    }
}