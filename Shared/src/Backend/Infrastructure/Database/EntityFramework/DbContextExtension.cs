namespace M47.Shared.Infrastructure.Database.EntityFramework;

using Microsoft.EntityFrameworkCore;
using System.Linq;

public static class DbContextExtension
{
    public static void DetachAllEntities(this DbContext context)
    {
        foreach (var dbEntityEntry in context.ChangeTracker.Entries().ToArray())
        {
            if (dbEntityEntry.Entity is not null)
            {
                dbEntityEntry.State = EntityState.Detached;
            }
        }
    }
}