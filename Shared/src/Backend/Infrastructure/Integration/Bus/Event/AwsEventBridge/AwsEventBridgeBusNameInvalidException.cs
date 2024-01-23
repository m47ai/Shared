namespace M47.Shared.Infrastructure.Integration.Bus.Event.AwsEventBridge;

using System;

[Serializable]
public class AwsEventBridgeBusNameInvalidException : Exception
{
    public AwsEventBridgeBusNameInvalidException(string? busName)
        : base($"Bus name '{busName}' is not valid")
    {
    }

    public AwsEventBridgeBusNameInvalidException() : base()
    {
    }

    public AwsEventBridgeBusNameInvalidException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}