namespace M47.Shared.Tests.Infrastructure.Integration.Bus.Event.AwsEventBridge;

using Amazon.EventBridge;
using M47.Shared.ConfigurationServices;
using M47.Shared.Domain.Bus.Event;
using M47.Shared.Infrastructure.Integration.Bus.Event;
using M47.Shared.Infrastructure.Integration.Bus.Event.AwsEventBridge;
using M47.Shared.Tests;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

public sealed class AwsEventBridgeBusTests : InjectionBaseTest
{
    private readonly IEventBus _eventBus;
    private readonly Fixture _fixture;

    public AwsEventBridgeBusTests()
    {
        _fixture = new();
        _eventBus = GetFromServices<IEventBus>();
    }

    [Fact(Skip = "Pending to create resources")]
    public async Task Should_SendEvent_When_MessageIsValid()
    {
        // Arrange
        var domainEvent = _fixture.CreateMany<DomainEvent>(1).ToList();

        // Act
        var results = await _eventBus.PublishAsync(domainEvent);

        // Assert
        Assert.True(results.Any());
    }

    [Fact(Skip = "Pending to create resources")]
    public async Task Should_ThrowException_When_NoValidBusName()
    {
        // Arrange
        var busNameConfiguration = Options.Create(new AwsEventBridgeConfiguration() { BusName = _fixture.Create<string>() });
        var checkNameEventBus = new AwsEventBridgeBus(Substitute.For<IAwsEventBridgePublisher>(), busNameConfiguration);
        var domainEvent = _fixture.CreateMany<DomainEvent>(1).ToList();

        // Act
        Func<Task> action = async () => await checkNameEventBus.PublishAsync(domainEvent);

        // Assert
        await action.Should().ThrowExactlyAsync<AwsEventBridgeBusNameInvalidException>();
    }

    protected override IServiceCollection ConfigureServiceProvider()
    {
        var services = new ServiceCollection();

        var configuration = GetConfiguration();
        services.AddLogging();
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        services.AddAWSService<IAmazonEventBridge>();
        services.Configure<AwsEventBridgeConfiguration>(configuration.GetSection("AwsEventBridgeConfiguration"));
        services.AddScoped<IAwsEventBridgePublisher, AwsEventBridgePublisher>();
        services.AddSingleton<DomainEventJsonSerializer>();
        services.AddRetryDecorator<IAwsEventBridgePublisher, AwsEventBridgePublisherRetry>();
        services.AddScoped<IEventBus, AwsEventBridgeBus>();

        return services;
    }
}