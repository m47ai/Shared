namespace M47.Shared.Infrastructure.Clock;

using System;

public interface IClockTime
{
    DateTime GetUtcNow();
}