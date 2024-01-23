namespace M47.Shared.Infrastructure.Database.Mysql;

using Dapper;
using MySqlConnector;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MysqlProvider : IMysqlProvider
{
    public async Task<IEnumerable<T>> QueryAsync<T>(string connectionString, string sql, object? @params = null)
    {
        using var connection = new MySqlConnection(connectionString);
        var result = await connection.QueryAsync<T>(sql, @params);

        return result;
    }

    public async Task<int> ExecuteAsync(string connectionString, string sql, object? @params = null)
    {
        using var connection = new MySqlConnection(connectionString);
        var result = await connection.ExecuteAsync(sql, @params);

        return result;
    }

    public async Task<int> ScalarAsync(string connectionString, string sql, object? @params = null)
    {
        using var connection = new MySqlConnection(connectionString);
        var result = await connection.ExecuteScalarAsync<int>(sql, @params);

        return result;
    }
}