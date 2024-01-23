namespace M47.Shared.Infrastructure.Database.EntityFramework.EfUtcDatime;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;

public static class UtcDateAnnotation
{
    private const string IsUtcAnnotation = "IsUtc";

    public static PropertyBuilder<TProperty> IsUtc<TProperty>(this PropertyBuilder<TProperty> builder, bool isUtc = true)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.HasAnnotation(IsUtcAnnotation, isUtc);
    }

    public static bool IsUtc(this IMutableProperty property)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        var attribute = property.PropertyInfo?.GetCustomAttribute<IsUtcAttribute>();
        if (attribute is not null && attribute.IsUtc)
        {
            return true;
        }

        return (bool?)property.FindAnnotation(IsUtcAnnotation)?.Value ?? true;
    }

    /// <summary>
    /// Make sure this is called after configuring all your entities.
    /// </summary>
    public static void ApplyUtcDateTimeConverter(this ModelBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (!property.IsUtc())
                {
                    continue;
                }

                if (property.ClrType == typeof(DateTime) ||
                    property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(typeof(UtcValueConverter));
                }
            }
        }
    }
}