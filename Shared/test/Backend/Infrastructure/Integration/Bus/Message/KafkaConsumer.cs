namespace M47.Shared.Tests.Infrastructure.Integration.Bus.Message;

using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

public class KafkaConsumer
{
    private readonly ILogger<KafkaConsumer> _logger;

    public KafkaConsumer(ILogger<KafkaConsumer> logger)
    {
        _logger = logger;
    }

    /// <summary>
    ///     In this example
    ///         - offsets are automatically committed.
    ///         - no extra thread is created for the Poll (Consume) loop.
    /// </summary>
    public IEnumerable<string> Consume(string brokerList, string groupId, string[] topics, CancellationToken cancellationToken = default)
    {
        var config = CreateConfig(brokerList, groupId);
        var consumer = CreateConsumer(config);

        consumer.Subscribe(topics);

        var values = new List<string>();

        try
        {
            while (true)
            {
                try
                {
                    var consumeResult = consumer.Consume(cancellationToken);
                    if (consumeResult.IsPartitionEOF)
                    {
                        _logger.LogInformation("Reached end of topic {topic}, partition {partition}, offset {offset}.", consumeResult.Topic, consumeResult.Partition, consumeResult.Offset);

                        continue;
                    }

                    values.Add(consumeResult.Message.Value);

                    _logger.LogInformation("Received message at {topicPartitionOffset}: {value}", consumeResult.TopicPartitionOffset, consumeResult.Message.Value);

                    try
                    {
                        // Store the offset associated with consumeResult to a local cache. Stored offsets are committed to Kafka by a background thread every AutoCommitIntervalMs.
                        // The offset stored is actually the offset of the consumeResult + 1 since by convention, committed offsets specify the next message to consume.
                        // If EnableAutoOffsetStore had been set to the default value true, the .NET client would automatically store offsets immediately prior to delivering messages to the application.
                        // Explicitly storing offsets after processing gives at-least once semantics, the default behavior does not.
                        consumer.StoreOffset(consumeResult);
                    }
                    catch (KafkaException e)
                    {
                        _logger.LogError("Store Offset error: {reason}", e.Error.Reason);
                    }
                }
                catch (ConsumeException e)
                {
                    _logger.LogError("Consume error: {reason}", e.Error.Reason);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Closing consumer.");
            consumer.Close();
        }

        return values;
    }

    private static ConsumerConfig CreateConfig(string brokerList, string groupId)
        => new()
        {
            BootstrapServers = brokerList,
            GroupId = groupId,
            EnableAutoOffsetStore = false,
            EnableAutoCommit = true,
            StatisticsIntervalMs = 5000,
            SessionTimeoutMs = 6000,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnablePartitionEof = true,
            // A good introduction to the CooperativeSticky assignor and incremental rebalancing:
            // https://www.confluent.io/blog/cooperative-rebalancing-in-kafka-streams-consumer-ksqldb/
            PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky
        };

    private IConsumer<Ignore, string> CreateConsumer(ConsumerConfig config)

        // Note: If a key or value deserializer is not set (as is the case below), the
        // deserializer corresponding to the appropriate type from Confluent.Kafka.Deserializers
        // will be used automatically (where available). The default deserializer for string
        // is UTF8. The default deserializer for Ignore returns null for all input data
        // (including non-null data).

        => new ConsumerBuilder<Ignore, string>(config)
            // Note: All handlers are called on the main .Consume thread.
            .SetErrorHandler((_, e) => _logger.LogError("Error: {reason}", e.Reason))
            //.SetStatisticsHandler((_, json) => _logger.LogDebug("Statistics: {stats}", json))
            .SetPartitionsAssignedHandler((c, partitions) =>
            {
                // Since a cooperative assignor (CooperativeSticky) has been configured, the
                // partition assignment is incremental (adds partitions to any existing assignment).
                _logger.LogDebug("Partitions incrementally assigned: [{assigned}], all: [{all}]",
                    string.Join(',', partitions.Select(p => p.Partition.Value)),
                    string.Join(',', c.Assignment.Concat(partitions).Select(p => p.Partition.Value)));

                // Possibly manually specify start offsets by returning a list of topic/partition/offsets
                // to assign to, e.g.:
                // return partitions.Select(tp => new TopicPartitionOffset(tp, externalOffsets[tp]));
            })
            .SetPartitionsRevokedHandler((c, partitions) =>
            {
                // Since a cooperative assignor (CooperativeSticky) has been configured, the revoked
                // assignment is incremental (may remove only some partitions of the current assignment).
                var remaining = c.Assignment.Where(atp => partitions.Where(rtp => rtp.TopicPartition == atp).Count() == 0);
                _logger.LogDebug("Partitions incrementally revoked: [{partitions}], remaining: [{remaing}]",
                                string.Join(',', partitions.Select(p => p.Partition.Value)),
                                string.Join(',', remaining.Select(p => p.Partition.Value)));
            })
            .SetPartitionsLostHandler((c, partitions) =>
            {
                // The lost partitions handler is called when the consumer detects that it has lost ownership
                // of its assignment (fallen out of the group).
                _logger.LogDebug("Partitions were lost: [{partitions}]", string.Join(", ", partitions));
            })
            .Build();

    /// <summary>
    ///     In this example
    ///         - consumer group functionality (i.e. .Subscribe + offset commits) is not used.
    ///         - the consumer is manually assigned to a partition and always starts consumption
    ///           from a specific offset (0).
    /// </summary>
    public void RunManualAssign(string brokerList, List<string> topics, CancellationToken cancellationToken)
    {
        var config = new ConsumerConfig
        {
            // the group.id property must be specified when creating a consumer, even
            // if you do not intend to use any consumer group functionality.
            GroupId = "groupid-not-used-but-mandatory",
            BootstrapServers = brokerList,
            // partition offsets can be committed to a group even by consumers not
            // subscribed to the group. in this example, auto commit is disabled
            // to prevent this from occurring.
            EnableAutoCommit = false
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config)
                                    .SetErrorHandler((_, e) => _logger.LogError("Error: {reason}", e.Reason))
                                    .Build();

        consumer.Assign(topics.Select(topic => new TopicPartitionOffset(topic, 0, Offset.Beginning)).ToList());

        try
        {
            while (true)
            {
                try
                {
                    var consumeResult = consumer.Consume(cancellationToken);
                    // Note: End of partition notification has not been enabled, so
                    // it is guaranteed that the ConsumeResult instance corresponds
                    // to a Message, and not a PartitionEOF event.
                    _logger.LogInformation("Received message at {topicPartitionOffset}: {value}", consumeResult.TopicPartitionOffset, consumeResult.Message.Value);
                }
                catch (ConsumeException e)
                {
                    _logger.LogError("Consume error: {reason}", e.Error.Reason);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Closing consumer.");
            consumer.Close();
        }
    }
}