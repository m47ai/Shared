namespace M47.Shared.Infrastructure.Database;

using System.Collections.Generic;
using System.Threading.Tasks;

public interface IDataBaseProvider
{
    Task<IEnumerable<T>> QueryAsync<T>(string connectionString, string sql, object? @params = null);

    Task<int> ExecuteAsync(string connectionString, string sql, object? @params = null);

    Task<int> ScalarAsync(string connectionString, string sql, object? @params = null);
}