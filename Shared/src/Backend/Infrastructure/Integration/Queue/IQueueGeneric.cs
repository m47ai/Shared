namespace M47.Shared.Infrastructure.Integration.Queue;

using System.Collections.Generic;
using System.Threading.Tasks;

public interface IQueue<T>
{
    Task<string> SendMessageAsync(string queueId, T message, int delayInSeconds = 0,
                                  CancellationToken cancellationToken = default);

    Task<string> DeleteMessageAsync(string queueId, string messageId, CancellationToken cancellationToken = default);

    Task<Dictionary<string, T>> PullMessagesAsync(string queueId, int waitTimeSeconds = 5, int nMessagesToRetrieve = 1,
                                                  CancellationToken cancellationToken = default);
}