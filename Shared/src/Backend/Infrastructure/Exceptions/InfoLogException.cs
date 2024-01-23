namespace M47.Shared.Infrastructure.Exceptions;

using System;

[Serializable]
public abstract class InfoLogException : Exception
{
    protected InfoLogException(string message, Exception e)
        : base(message, e)
    {
    }

    protected InfoLogException() : base()
    {
    }

    protected InfoLogException(string? message) : base(message)
    {
    }
}