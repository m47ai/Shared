namespace M47.Shared.Domain.Bus.Event;

using System;

public abstract class DomainEvent : IBaseEvent
{
    private readonly Guid _eventId;
    private readonly DateTime _occurredOn;

    protected DomainEvent(Guid eventId, DateTime occurredOn)
    {
        _eventId = eventId;
        _occurredOn = occurredOn;
    }

    public Guid GetEventId() => _eventId;

    public DateTime GetOccurredOn() => _occurredOn;

    public abstract string GetFullQualifiedEventName();
}