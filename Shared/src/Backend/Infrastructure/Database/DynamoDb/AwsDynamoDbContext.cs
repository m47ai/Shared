namespace M47.Shared.Infrastructure.Database.DynamoDb;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using M47.Shared.Domain.Configurations;
using M47.Shared.Utils.Disposable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class DynamoDbContext<T> : Disposable, IDynamoDbContext<T> where T : class
{
    // https://underscorehao.net/2018/03/net-core-api-dynamodb-dependency-injection/
    private const int _batchLimit = 100;

    private readonly DynamoDBOperationConfig _config;
    private readonly IDynamoDBContext _context;

    public DynamoDbContext(DynamoDbConfig config, string tableName, string? aws_access_key_id = null,
                           string? aws_secret_access_key = null, string? region = null)
    {
        var clientConfig = new AmazonDynamoDBConfig()
        {
            Timeout = TimeSpan.FromSeconds(config.TimeOutInSeconds),
            MaxErrorRetry = config.MaxErrorRetry,
            RegionEndpoint = Amazon.RegionEndpoint.EUWest1,
        };

        _config = new DynamoDBOperationConfig()
        {
            OverrideTableName = tableName
        };

        var client = GetDynamoDbClient(aws_access_key_id, aws_secret_access_key, region, clientConfig);

        _context = new DynamoDBContext(client);
    }

    private static AmazonDynamoDBClient GetDynamoDbClient(string? aws_access_key_id, string? aws_secret_access_key,
                                                          string? region, AmazonDynamoDBConfig clientConfig)
    {
        if (HasValidAwsConfiguration(aws_access_key_id, aws_secret_access_key, region))
        {
            return new AmazonDynamoDBClient(clientConfig);
        }

        clientConfig.RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region);

        return new AmazonDynamoDBClient(aws_access_key_id, aws_secret_access_key, clientConfig);
    }

    private static bool HasValidAwsConfiguration(string? aws_access_key_id, string? aws_secret_access_key, string? region)
    {
        return String.IsNullOrEmpty(aws_access_key_id) ||
               String.IsNullOrEmpty(aws_secret_access_key) ||
               String.IsNullOrEmpty(region);
    }

    public override void DisposeManagedResources() => _context.Dispose();

    public async Task<T> GetAsync(T key, CancellationToken cancellationToken = default)
        => await _context.LoadAsync(key, _config, cancellationToken);

    public async Task<T> GetAsync(object key, CancellationToken cancellationToken = default)
        => await _context.LoadAsync<T>(key, _config, cancellationToken);

    public async Task<IEnumerable<T>> BatchGetAsync(IEnumerable<object> keys, CancellationToken cancellationToken = default)
    {
        var results = new List<T>();
        var totalKeys = keys.Count();
        var pageNumber = 0;

        while (totalKeys > 0)
        {
            var partialKeys = keys.Skip(_batchLimit * pageNumber++).Take(_batchLimit);

            var partialResult = await StepBatchGetAsync(partialKeys, cancellationToken);

            results.AddRange(partialResult);

            totalKeys -= partialKeys.Count();
        }

        return results;
    }

    public async Task SaveAsync(T document, CancellationToken cancellationToken = default)
        => await _context.SaveAsync(document, _config, cancellationToken);

    public async Task BatchSaveAsync(IEnumerable<T> documents, CancellationToken cancellationToken = default)
    {
        var totalDocuments = documents.Count();
        var pageNumber = 0;

        while (totalDocuments > 0)
        {
            var partialDocuments = documents.Skip(_batchLimit * pageNumber++).Take(_batchLimit);

            await StepBatchSaveAsync(partialDocuments, cancellationToken);

            totalDocuments -= partialDocuments.Count();
        }
    }

    public async Task DeleteAsync(T document, CancellationToken cancellationToken = default)
        => await _context.DeleteAsync(document, _config, cancellationToken);

    public async Task DeleteAsync(object key, CancellationToken cancellationToken = default)
        => await _context.DeleteAsync(key, _config, cancellationToken);

    public async Task BatchDeleteAsync(IEnumerable<T> documents, CancellationToken cancellationToken = default)
    {
        var batchWrite = _context.CreateBatchWrite<T>(_config);
        batchWrite.AddDeleteItems(documents);

        await batchWrite.ExecuteAsync(cancellationToken);
    }

    public async Task BatchDeleteAsync(IEnumerable<object> keys, CancellationToken cancellationToken = default)
    {
        var batchWrite = _context.CreateBatchWrite<T>(_config);
        foreach (var key in keys)
        {
            batchWrite.AddDeleteKey(key);
        }

        await batchWrite.ExecuteAsync(cancellationToken);
    }

    private async Task<IEnumerable<T>> StepBatchGetAsync(IEnumerable<object> keys, CancellationToken cancellationToken)
    {
        var batchRead = _context.CreateBatchGet<T>(_config);

        foreach (var key in keys)
        {
            batchRead.AddKey(key);
        }

        await batchRead.ExecuteAsync(cancellationToken);

        return batchRead.Results;
    }

    private async Task StepBatchSaveAsync(IEnumerable<T> documents, CancellationToken cancellationToken)
    {
        var batchWrite = _context.CreateBatchWrite<T>(_config);
        batchWrite.AddPutItems(documents);

        await batchWrite.ExecuteAsync(cancellationToken);
    }
}