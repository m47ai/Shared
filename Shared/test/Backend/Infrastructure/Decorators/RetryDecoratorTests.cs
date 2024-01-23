namespace M47.Shared.Tests.Infrastructure.Decorators;

using Amazon.EventBridge;
using M47.Shared.Domain.Bus.Event;
using M47.Shared.Domain.Messages;
using M47.Shared.Infrastructure.Decorators;
using M47.Shared.Infrastructure.Integration.Bus.Event.AwsEventBridge;
using NSubstitute.ExceptionExtensions;
using System;
using System.Linq;
using System.Threading.Tasks;

public sealed class RetryDecoratorTests : BaseTest
{
    private readonly int _retryAttempts = 3;
    private readonly IAwsEventBridgePublisher _retryPublisher;
    private readonly IAwsEventBridgePublisher _publisher;
    private readonly Fixture _fixture;

    public RetryDecoratorTests()
    {
        _fixture = new();
        _publisher = Substitute.For<IAwsEventBridgePublisher>();
        _retryPublisher = new AwsEventBridgePublisherRetry(_publisher, new RetryDecorator(), TimeSpan.FromSeconds(0),
                                                           _retryAttempts);
    }

    [Fact(Skip = "Pending to create resources")]
    public async Task Should_RetryPublish_When_IsTransientException()
    {
        // Arrange
        SetupAwsEventBridgePublisherToThrow<AmazonEventBridgeException>();

        // Act
        Task<string> action() => _retryPublisher.SendAsync(_fixture.Create<string>(), _fixture.Create<Message<DomainEvent>>());

        // Assert
        await ShouldRetry(action);
    }

    [Fact(Skip = "Pending to create resources")]
    public async Task Should_NoRetry_When_IsNoTransientException()
    {
        // Arrange
        SetupAwsEventBridgePublisherToThrow<ArgumentException>();

        // Act
        Task<string> action() => _retryPublisher.SendAsync(_fixture.Create<string>(), _fixture.Create<Message<DomainEvent>>());

        // Assert
        await ShoulNotRetry(action);
    }

    private async Task ShouldRetry(Func<Task<string>> action)
    {
        // Arrange
        var results = await Assert.ThrowsAsync<AggregateException>(action);

        // Act
        Assert.True(results.InnerExceptions.All(x => x is AmazonEventBridgeException));

        // Assert
        await _publisher.Received(_retryAttempts).SendAsync(Arg.Any<string>(), Arg.Any<Message<DomainEvent>>());
    }

    private async Task ShoulNotRetry(Func<Task<string>> action)
    {
        // Act
        _ = await Assert.ThrowsAsync<ArgumentException>(action);

        // Assert
        await _publisher.Received(1).SendAsync(Arg.Any<string>(), Arg.Any<Message<DomainEvent>>());
    }

    private void SetupAwsEventBridgePublisherToThrow<T>() where T : Exception
        => _publisher.SendAsync(Arg.Any<string>(), Arg.Any<Message<DomainEvent>>())!
                     .ThrowsAsync((T)Activator.CreateInstance(typeof(T), typeof(T).ToString())!);
}