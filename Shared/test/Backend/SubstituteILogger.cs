namespace M47.Shared.Tests;

using Microsoft.Extensions.Logging;
using System;

// see https://stackoverflow.com/questions/46529349/nsubstitute-ilogger-net-core
public abstract class SubstituteILogger<T> : ILogger<T>
{
    void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter) =>
        Log(logLevel, formatter(state, exception!));

    public abstract void Log(LogLevel logLevel, string message);

    public virtual bool IsEnabled(LogLevel logLevel) => true;

    public abstract IDisposable? BeginScope<TState>(TState state) where TState : notnull;
}