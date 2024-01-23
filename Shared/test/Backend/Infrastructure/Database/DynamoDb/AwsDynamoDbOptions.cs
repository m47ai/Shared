namespace M47.Shared.Tests.Infrastructure.Database.DynamoDb;

using M47.Shared.Domain.Configurations;

public class AwsDynamoDbOptions
{
    public string? TableName { get; set; }
    public DynamoDbConfig? DynamoDbConfig { get; set; }
}