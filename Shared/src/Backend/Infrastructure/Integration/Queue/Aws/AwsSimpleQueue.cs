namespace M47.Shared.Infrastructure.Integration.Queue.Aws;

using Amazon.SQS;
using Amazon.SQS.Model;
using M47.Shared.Infrastructure.Integration.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AwsSimpleQueue : IQueue
{
    private readonly IAmazonSQS _client;

    public AwsSimpleQueue(IAmazonSQS client)
    {
        _client = client;
    }

    public async Task<string> SendMessageAsync(string queueId, string message, int delayInSeconds = 0,
                                               CancellationToken cancellationToken = default)
    {
        var request = new SendMessageRequest
        {
            QueueUrl = await GetQueueUrlAsync(queueId, cancellationToken).ConfigureAwait(false),
            MessageBody = message,
            DelaySeconds = delayInSeconds
        };

        var response = await _client.SendMessageAsync(request, cancellationToken)
                                    .ConfigureAwait(false); ;

        return response.MessageId;
    }

    public async Task<Dictionary<string, string>> PullMessagesAsync(string queueId, int waitTimeSeconds = 5,
                                                                    int nMessagesToRetrieve = 1,
                                                                    CancellationToken cancellationToken = default)
    {
        var dictMessages = new Dictionary<string, string>();
        var response = await _client.ReceiveMessageAsync(new ReceiveMessageRequest
        {
            QueueUrl = await GetQueueUrlAsync(queueId, cancellationToken).ConfigureAwait(false),
            MaxNumberOfMessages = nMessagesToRetrieve,
            WaitTimeSeconds = waitTimeSeconds
        }, cancellationToken);

        foreach (var message in response.Messages)
        {
            dictMessages.Add(message.ReceiptHandle, message.Body);
        }

        return dictMessages;
    }

    public async Task<string> DeleteMessageAsync(string queueId, string messageId,
                                                 CancellationToken cancellationToken = default)
    {
        var request = new DeleteMessageRequest
        {
            QueueUrl = await GetQueueUrlAsync(queueId, cancellationToken),
            ReceiptHandle = messageId
        };

        var response = await _client.DeleteMessageAsync(request, cancellationToken)
                                    .ConfigureAwait(false);

        return JsonConvert.SerializeObject(response);
    }

    private async Task<string> GetQueueUrlAsync(string queueId, CancellationToken cancellationToken = default)
    {
        if (Uri.IsWellFormedUriString(queueId, UriKind.Absolute))
        {
            return queueId;
        }

        var response = await _client.GetQueueUrlAsync(queueId, cancellationToken)
                                    .ConfigureAwait(false);

        return response.QueueUrl;
    }
}