namespace M47.Shared.Infrastructure.Integration.Queue.Aws;

using M47.Shared.Infrastructure.Integration.Queue;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AwsSimpleQueue<T> : IQueue<T> where T : class
{
    private readonly IQueue _queue;

    public AwsSimpleQueue(IQueue queue)
    {
        _queue = queue;
    }

    public async Task<string> SendMessageAsync(string queueId, T message, int delayInSeconds = 0,
                                               CancellationToken cancellationToken = default)
    {
        var serializedMessage = JsonConvert.SerializeObject(message, _camelCaseJsonSerializer);

        var messageId = await _queue.SendMessageAsync(queueId, serializedMessage, delayInSeconds, cancellationToken);

        return messageId;
    }

    public async Task<Dictionary<string, T>> PullMessagesAsync(string queueId, int waitTimeSeconds = 5,
                                                               int nMessagesToRetrieve = 1,
                                                               CancellationToken cancellationToken = default)
    {
        var deserializedMessages = new Dictionary<string, T>();
        var messages = await _queue.PullMessagesAsync(queueId, waitTimeSeconds, nMessagesToRetrieve, cancellationToken);

        foreach (var message in messages)
        {
            deserializedMessages.Add(message.Key, JsonConvert.DeserializeObject<T>(message.Value)!);
        }

        return deserializedMessages;
    }

    public async Task<string> DeleteMessageAsync(string queueId, string id,
                                                 CancellationToken cancellationToken = default)
    {
        return await _queue.DeleteMessageAsync(queueId, id, cancellationToken);
    }

    private static JsonSerializerSettings _camelCaseJsonSerializer
        => new()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            },
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            Converters =
            {
                new StringEnumConverter(typeof(CamelCaseNamingStrategy))
            }
        };
}