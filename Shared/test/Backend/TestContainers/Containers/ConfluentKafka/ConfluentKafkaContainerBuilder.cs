namespace M47.Shared.Tests.TestContainers.Containers.ConfluentKafka;

using DotNet.Testcontainers.Builders;
using System.Threading;
using Testcontainers.Kafka;

public sealed class ConfluentKafkaContainerBuilder
{
    private readonly KafkaContainer _container;

    public ConfluentKafkaContainerBuilder(string group)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        _container = new KafkaBuilder()
            .WithImage("confluentinc/cp-kafka:7.3.0")
            .WithName($"{group.ToLower()}-kafka-{Guid.NewGuid()}")
            .WithOutputConsumer(Consume.RedirectStdoutAndStderrToConsole())
            .Build();
#pragma warning restore CS0618 // Type or member is obsolete
    }

    public string BootstrapServer() => _container.GetBootstrapAddress();

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await _container.StartAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task StopAsync()
    {
        await _container.StopAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}