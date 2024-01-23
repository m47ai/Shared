namespace M47.Shared.Domain.Aggregate;

using M47.Shared.Domain.Bus.Event;
using Newtonsoft.Json;
using System.Collections.Generic;

public abstract class AggregateRoot
{
    private List<DomainEvent> _domainEvents = new();

    public List<DomainEvent> PullDomainEvents()
    {
        List<DomainEvent> events = _domainEvents;

        _domainEvents = new List<DomainEvent>();

        return events;
    }

    protected void Record(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }
}