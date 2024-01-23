namespace M47.Shared.Tests.TestContainers.Containers.MySql;

using Testcontainers.MySql;

public sealed class MySqlContainerBuilder
{
    public readonly MySqlContainer Container;

    public MySqlContainerBuilder(string group, string database)
    {
        Container = new MySqlBuilder()
            .WithImage("mysql:8.0.28")
            .WithDatabase(database)
            .WithName($"{group.ToLower()}-mysql-{Guid.NewGuid()}")
            .Build();
    }
}