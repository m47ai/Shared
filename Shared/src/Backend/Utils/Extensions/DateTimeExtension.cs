namespace M47.Shared.Utils.Extensions;

using System;

public static class DateTimeExtension
{
    public static string ToMySql(this DateTime date) => date.ToString("yyyy-MM-dd HH:mm:ss.fff");

    public static string ToKeyName(this DateTime date) => date.ToString("yyyyMMdd-HHmmss");
}