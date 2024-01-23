namespace M47.Shared.Infrastructure.Integration.Queue;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public interface IScheduledQueue : IDisposable
{
    Task SendScheduledMessageAsync(string json, DateTime? scheduledDate);

    Task<IDictionary<object, string>?> GetMessageAsync(int maxMessagesCount, int timeout,
                                                       CancellationToken cancellationToken = default);

    Task DeleteMessageAsync(object message);
}