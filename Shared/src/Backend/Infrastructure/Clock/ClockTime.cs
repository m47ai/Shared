namespace M47.Shared.Infrastructure.Clock;

using System;

public class ClockTime : IClockTime
{
    public DateTime GetUtcNow()
    {
        return DateTime.UtcNow;
    }
}