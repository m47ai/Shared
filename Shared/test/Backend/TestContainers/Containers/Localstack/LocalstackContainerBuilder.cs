namespace M47.Shared.Tests.TestContainers.Containers.Localstack;

using Amazon.Runtime;
using Castle.Core.Logging;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using M47.Shared.Tests.Utils.Network;
using M47.Shared.Utils.Files;
using Testcontainers.LocalStack;

public sealed class LocalstackContainerBuilder<TEntryPoint> where TEntryPoint : class
{
    private const int _localstackInternalPort = 4566;

    private readonly int _exposedPort = Ports.GetAvailablePort();

    public readonly BasicAWSCredentials Credentials = new(AwsFakeCredentials.AwsAccessKey, AwsFakeCredentials.AwsSecretKey);
    public LocalStackContainer Container { get; }

    public LocalstackContainerBuilder(string group)
    {
        var waitStrategy = Wait.ForUnixContainer()
                               .UntilPortIsAvailable(_localstackInternalPort)
                               .AddCustomWaitStrategy(new LocalStackHealthCheck(_localstackInternalPort));

        Container = new LocalStackBuilder()
            .WithImage("localstack/localstack:latest")
            .WithEnvironment("AWS_ACCESS_KEY_ID", AwsFakeCredentials.AwsAccessKey)
            .WithEnvironment("AWS_SECRET_ACCESS_KEY", AwsFakeCredentials.AwsAccessKey)
            .WithName($"{group.ToLower()}-localstack-{Guid.NewGuid()}")
            .WithPortBinding(_exposedPort, _localstackInternalPort)
            .WithBindMount(ProjectFiles.FromSolutionPath<TEntryPoint>("/scripts/development/localstack/"),
                                                                      "/etc/localstack/init/ready.d",
                                                                      AccessMode.ReadOnly)
            .WithWaitStrategy(waitStrategy)
            .Build();
    }

    public AwsLocalHelper AwsHelper => new(Uri);

    public string Uri => Container.GetConnectionString();
    //public string Uri => $"http://localhost:{_exposedPort}/";

    public async Task DebugConsoleAsync()
    {
        var logger = new ConsoleLogger();
        var (stdout, stderr) = await Container.GetLogsAsync();

        logger.Info(stdout);
        logger.Error(stderr);
    }
}