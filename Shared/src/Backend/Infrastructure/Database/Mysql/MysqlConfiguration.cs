namespace M47.Shared.Infrastructure.Database.Mysql;

using System;
using System.Linq;

public class MysqlConfiguration
{
    public string ConnectionString { get; }
    public string DatabaseName => GetDatabaseName();

    public MysqlConfiguration(string connectionString)
    {
        ConnectionString = connectionString;
    }

    private string GetDatabaseName()
    {
        var connectionStringParts = ConnectionString.Split(";");
        var databasePart = connectionStringParts.Where(x => x.Contains("database", StringComparison.CurrentCulture));
        if (!databasePart.Any())
        {
            throw new InvalidOperationException("Database part does not exists in the connection string");
        }

        var databaseName = databasePart.First().Split("=").Last();
        if (IsNotStaging(databaseName))
        {
            throw new InvalidOperationException("Database is not available for testing");
        }

        return databaseName;
    }

    private static bool IsNotStaging(string databaseName) => databaseName.IndexOf("test") < 0;
}