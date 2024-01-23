namespace M47.Shared.Domain.Configurations;

public class DynamoDbConfig
{
    public int TimeOutInSeconds { get; set; }
    public int MaxErrorRetry { get; set; }
}