namespace M47.Shared.Tests.Infrastructure.Database.DynamoDb;

using Amazon.DynamoDBv2.DataModel;
using M47.Shared.Infrastructure.Database.DynamoDb;
using M47.Shared.Infrastructure.Database.DynamoDb.Model;
using M47.Shared.Tests;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class AwsDynamoDbTests : InjectionBaseTest
{
    private readonly IDynamoDbContext<TestTableDynamoDb> _context;
    private readonly Fixture _fixture;

    public AwsDynamoDbTests()
    {
        _fixture = new();
        _context = GetFromServices<IDynamoDbContext<TestTableDynamoDb>>();
    }

    [Fact(Skip = "Pending to create resources")]
    public async Task Should_SaveGetAndDelete_When_DocumentIsValid()
    {
        // Arrange
        var expected = _fixture.Create<TestTableDynamoDb>();

        // Act
        await _context.SaveAsync(expected);
        var actual = await _context.GetAsync(expected);
        await _context.DeleteAsync(actual);

        // Assert
        actual.Id.Should().Be(expected.Id);
        actual.Applications!.Count.Should().Be(expected.Applications!.Count);
        actual.CreatedAt.Should().Be(expected.CreatedAt);
    }

    [Fact(Skip = "Pending to create resources")]
    public async Task Should_SaveGetAndDeleteInBatch_When_DocumentsAreValids()
    {
        // Arrange
        var expected = _fixture.CreateMany<TestTableDynamoDb>();

        // Act
        await _context.BatchSaveAsync(expected);
        var actual = await _context.BatchGetAsync(expected.Select(x => x.GetKey()));
        await _context.BatchDeleteAsync(actual);

        // Assert
        actual.Count().Should().Be(expected.Count());
        foreach (var expectedItem in expected)
        {
            var actualItem = actual.Single(actual => expectedItem.Id == actual.Id);
            actualItem!.Id.Should().Be(expectedItem.Id);
            actualItem.Applications!.Count.Should().Be(expectedItem.Applications!.Count);
            actualItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
        }
    }

    protected override IServiceCollection ConfigureServiceProvider()
    {
        var services = new ServiceCollection();

        var configuration = GetConfiguration();
        var awsOptions = configuration.GetAWSOptions();
        services.AddDefaultAWSOptions(awsOptions);

        var dynamoDbOptions = new AwsDynamoDbOptions();
        configuration.GetSection("DynamoDbTables").Bind(dynamoDbOptions);

        IDynamoDbContext<TestTableDynamoDb> dynamoDbFactory(IServiceProvider _)
            => new DynamoDbContext<TestTableDynamoDb>(dynamoDbOptions.DynamoDbConfig!, dynamoDbOptions.TableName!);

        services.AddScoped(dynamoDbFactory);

        return services;
    }

    private class TestTableDynamoDb : DocumentDynamoDb
    {
        public enum Application
        {
            A,
            B
        }

        [DynamoDBHashKey()]
        public long Id { get; set; }

        public DateTime CreatedAt { get; set; }
        public List<Application>? Applications { get; set; }
    }
}