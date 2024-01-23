namespace M47.Shared.Infrastructure.Integration.Bus.Event.AwsEventBridge;

using Amazon.EventBridge;
using M47.Shared.Domain.Bus.Event;
using M47.Shared.Domain.Messages;
using M47.Shared.Infrastructure.Decorators;
using System;
using System.Threading.Tasks;

public class AwsEventBridgePublisherRetry : IAwsEventBridgePublisher
{
    private const int _defaultRetryAttempts = 3;
    private static readonly TimeSpan _defaultRetryInterval = TimeSpan.FromSeconds(2);

    private readonly IAwsEventBridgePublisher _publisher;
    private readonly RetryDecorator _retry;
    private readonly TimeSpan _retryInterval;
    private readonly int _retryAttempts;

    public AwsEventBridgePublisherRetry(IAwsEventBridgePublisher publisher, RetryDecorator retry)
        : this(publisher, retry, _defaultRetryInterval, _defaultRetryAttempts)
    {
    }

    public AwsEventBridgePublisherRetry(IAwsEventBridgePublisher publisher, RetryDecorator retry,
                                        TimeSpan retryInterval, int retryAttempts)
    {
        _publisher = publisher;
        _retry = retry;
        _retryInterval = retryInterval;
        _retryAttempts = retryAttempts;
    }

    public async Task<string> SendAsync(string exchangeName, Message<DomainEvent> message)
    {
        return await _retry.Execute(() => _publisher.SendAsync(exchangeName, message),
                                    (Exception ex) => IsTransient(ex),
                                    _retryAttempts,
                                    _retryInterval);
    }

    private static bool IsTransient(Exception ex) => ex is AmazonEventBridgeException;
}