namespace M47.Shared.Infrastructure.Database.DynamoDb;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public interface IDynamoDbContext<T> where T : class
{
    Task<T> GetAsync(T key, CancellationToken cancellationToken = default);

    Task<T> GetAsync(object key, CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> BatchGetAsync(IEnumerable<object> keys, CancellationToken cancellationToken = default);

    Task SaveAsync(T document, CancellationToken cancellationToken = default);

    Task BatchSaveAsync(IEnumerable<T> documents, CancellationToken cancellationToken = default);

    Task DeleteAsync(T document, CancellationToken cancellationToken = default);

    Task DeleteAsync(object key, CancellationToken cancellationToken = default);

    Task BatchDeleteAsync(IEnumerable<T> documents, CancellationToken cancellationToken = default);

    Task BatchDeleteAsync(IEnumerable<object> keys, CancellationToken cancellationToken = default);
}