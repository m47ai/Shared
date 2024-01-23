namespace M47.Shared.Utils.Identifiers;

/// <summary>
/// Initialization code for production:
///     SystemGuid.Init(() => Guid.NewGuid());
///
/// Initialization code for unit tests:
///     SystemGuid.Init(() => new Guid("0a9bc91d-28cd-4e37-a2bd-dcf454290aa8"));
/// </summary>

public static class SystemGuid
{
    private static Func<Guid> _func = () => Guid.NewGuid();

    public static Guid NewGuid() => _func();

    public static void Init(Func<Guid> func) => _func = func;

    public static void DefaultInit() => _func = () => Guid.NewGuid();
}