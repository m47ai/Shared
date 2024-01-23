namespace M47.Shared.Infrastructure.Database.EntityFramework.EfUtcDatime;

[AttributeUsage(AttributeTargets.Property)]
public class IsUtcAttribute : Attribute
{
    public IsUtcAttribute(bool isUtc = true) => IsUtc = isUtc;

    public bool IsUtc { get; }
}