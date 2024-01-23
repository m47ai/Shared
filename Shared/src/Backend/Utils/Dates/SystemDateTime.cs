namespace M47.Shared.Utils.Dates;

/// <summary>
/// Initialization code for production:
///     SystemDateTime.Init(() => DateTime.UtcNow);
///
/// Initialization code for unit tests:
///     SystemDateTime.Init(() => new DateTime(2016, 5, 3));
/// </summary>
public static class SystemDateTime
{
    private static Func<DateTime> _func = () => DateTime.UtcNow;

    public static DateTime UtcNow => _func();

    public static void Init(Func<DateTime> func) => _func = func;

    public static void DefaultInit() => _func = () => DateTime.UtcNow;
}