namespace M47.Shared.Infrastructure.Database.EntityFramework;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public static class ModelBuilderExtension
{
    public static void NoPluralizeTables(this ModelBuilder modelBuilder)
    {
        // This will singularize all table names. Before define OwnedTypes
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!entityType.IsOwned())
            {
                modelBuilder.Entity(entityType.ClrType).ToTable(entityType.ClrType.Name);
                entityType.SetTableName(entityType.DisplayName());
            }
        }
    }

    /// <summary>
    /// Prefix primary column names with table name (Id => UserId)
    /// </summary>
    public static void PrimaryKeyToTableNameId(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.Name == "Id" && !entityType.IsOwned())
                {
                    property.SetColumnName(entityType.DisplayName() + "Id");
                }
            }
        }
    }

    public static void FixEntity<T>(this EntityEntry entry, string reference, object value)
    {
        if (entry.Entity is T)
        {
            if (entry.Reference(reference).CurrentValue is null)
            {
                entry.Reference(reference).CurrentValue = value;
            }

            entry.Reference(reference).TargetEntry!.State = entry.State;
        }
    }
}