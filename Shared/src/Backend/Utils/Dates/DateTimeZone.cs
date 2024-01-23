namespace M47.Shared.Utils.Dates;

using System;
using TimeZoneConverter;

public static class DateTimeZone
{
    public static DateTime ConvertToZone(DateTime utcDate, string timezone)
    {
        var tz = TZConvert.GetTimeZoneInfo(timezone);

        var offset = tz.GetUtcOffset(utcDate);

        return utcDate.AddHours(offset.Hours);
    }

    public static DateTime ChangeTimezone(DateTime date, string sourceTimezone, string destTimezone = "Etc/GMT")
    {
        var localDate = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Millisecond, DateTimeKind.Unspecified);
        var sourceTimeZoneInfo = TZConvert.GetTimeZoneInfo(sourceTimezone);
        var destinationTimeZoneInfo = TZConvert.GetTimeZoneInfo(destTimezone);

        return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(localDate, sourceTimeZoneInfo.Id, destinationTimeZoneInfo.Id);
    }
}