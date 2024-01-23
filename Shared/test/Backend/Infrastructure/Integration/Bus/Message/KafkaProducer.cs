namespace M47.Shared.Tests.Infrastructure.Integration.Bus.Message;

using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

public class KafkaProducer
{
    private readonly ILogger<KafkaProducer> _logger;
    private readonly string _bootstrapServers;

    public KafkaProducer(string bootstrapServers, ILogger<KafkaProducer> logger)
    {
        _logger = logger;
        _bootstrapServers = bootstrapServers;
    }

    public async Task CreateTopicAsync(string topicName)
    {
        using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = _bootstrapServers }).Build();

        try
        {
            var topicSpecification = new TopicSpecification[] {
                new TopicSpecification
                {
                    Name = topicName, ReplicationFactor = 1, NumPartitions = 1
                }
            };

            await adminClient.CreateTopicsAsync(topicSpecification);
        }
        catch (CreateTopicsException e)
        {
            _logger.LogError("An error occurred creating topic {topic}: {reason}", e.Results[0].Topic, e.Results[0].Error.Reason);
        }
    }

    public async Task DeleteTopicAsync(params string[] topicsName)
    {
        using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = _bootstrapServers }).Build();

        try
        {
            await adminClient.DeleteTopicsAsync(topicsName);
        }
        catch (DeleteTopicsException e)
        {
            _logger.LogError("An error occurred deleting topic {topic}: {reason}", e.Results[0].Topic, e.Results[0].Error.Reason);
        }
    }

    public void Produce<TKey, TValue>(string topicName,
                                      IDictionary<TKey, TValue> messages,
                                      CancellationToken cancellationToken = default)
    {
        var config = new ProducerConfig { BootstrapServers = _bootstrapServers };

        using var producer = new ProducerBuilder<TKey, TValue>(config).Build();

        var tasks = CreateTasks(topicName, messages, producer, cancellationToken);

        Task.WaitAll(tasks, cancellationToken);

        producer.Flush(TimeSpan.FromSeconds(10));
    }

    private Task<DeliveryResult<TKey, TValue>>[] CreateTasks<TKey, TValue>(string topicName,
                                                                           IDictionary<TKey, TValue> messages,
                                                                           IProducer<TKey, TValue> producer,
                                                                           CancellationToken cancellationToken)
        => messages.Select(x =>
        {
            var message = new Message<TKey, TValue> { Key = x.Key, Value = x.Value };

            return CreateTask(topicName, message, producer, cancellationToken);
        }).ToArray();

    private Task<DeliveryResult<TKey, TValue>> CreateTask<TKey, TValue>(string topicName,
                                                                        Message<TKey, TValue> message,
                                                                        IProducer<TKey, TValue> producer,
                                                                        CancellationToken cancellationToken)
    {
        var task = producer.ProduceAsync(topicName, message, cancellationToken);

        task.ContinueWith(antecedent =>
        {
            if (antecedent.Status == TaskStatus.RanToCompletion)
            {
                _logger.LogInformation("Produced event to topic {topicName}: key = {key} value = {value}", topicName, message.Key, message.Value);
            }

            if (antecedent.Status == TaskStatus.Faulted)
            {
                var ex = antecedent.Exception!.GetBaseException();

                ErrorCode? errorCode = null;

                if (ex.GetType() == typeof(ProduceException<TKey, TValue>))
                {
                    errorCode = ((ProduceException<TKey, TValue>)ex).Error.Code;
                }

                _logger.LogError("Failed to deliver message: {message} [{errorCode}]", ex.Message, errorCode);
            }
        });

        return task;
    }
}