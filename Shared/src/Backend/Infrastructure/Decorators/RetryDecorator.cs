namespace M47.Shared.Infrastructure.Decorators;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

public class RetryDecorator
{
    private const double _defaultFactorBackoffInterval = 1.25;

    public T Execute<T>(Func<T> action, Func<Exception, bool> isTransient, int maxAttemptCount,
                        TimeSpan retryInterval, double factor = _defaultFactorBackoffInterval)
    {
        var exceptions = new List<Exception>();
        int backoffInterval;

        for (int attempted = 0; attempted < maxAttemptCount; attempted++)
        {
            try
            {
                return action();
            }
            catch (Exception ex) when (IsTransient(ex, isTransient))
            {
                exceptions.Add(ex);
            }

            // Wait to retry the operation.
            // Consider calculating an exponential delay here and
            // using a strategy best suited for the operation and fault.
            backoffInterval = (int)(retryInterval.TotalMilliseconds * factor);

            Thread.Sleep(backoffInterval);
        }

        throw new AggregateException($"Exceed the max({maxAttemptCount}) retries for {nameof(action)}", exceptions);
    }

    private static bool IsTransient(Exception ex, Func<Exception, bool> isTransient)
    {
        // Determine if the exception is transient.
        // In some cases this is as simple as checking the exception type, in other
        // cases it might be necessary to inspect other properties of the exception.
        if (isTransient(ex))
        {
            return true;
        }

        if (ex is WebException webException)
        {
            // If the web exception contains one of the following status values
            // it might be transient.
            return new[] {
                WebExceptionStatus.ConnectionClosed,
                WebExceptionStatus.Timeout,
                WebExceptionStatus.RequestCanceled
            }.Contains(webException.Status);
        }

        return false;
    }
}