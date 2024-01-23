namespace M47.Shared.Domain.Bus.Event;

using System.Collections.Generic;
using System.Threading.Tasks;

public interface IEventBus
{
    Task<IEnumerable<string>> PublishAsync(List<DomainEvent> events);
}