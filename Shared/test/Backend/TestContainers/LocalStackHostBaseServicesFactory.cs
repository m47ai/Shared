namespace M47.Shared.Tests.TestContainers;

using Bogus;
using M47.Shared.Tests.TestContainers.Containers.Localstack;
using M47.Shared.Tests.Utils.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using VerifyTests;
using WireMock.Server;

public abstract class LocalStackHostBaseServicesFactory<TEntryPoint> : IAsyncLifetime where TEntryPoint : BackgroundService
{
    public readonly LocalstackContainerBuilder<TEntryPoint> Localstack;
    protected readonly WireMockServer WireMockServer;

    public LocalStackHostBaseServicesFactory(string group)
    {
        Randomizer.Seed = new Random(666);
        VerifierSettings.ScrubInlineGuids();

        Localstack = new(group);

        WireMockServer = WireMockServer.Start(Ports.GetAvailablePort());
    }

    public IHost CreateDefaultHost() => CreateDefaultHostBuilder().Build();

    public T GetService<T>(IHost host) where T : class => host.Services.GetService<T>()!;

    public async Task InitializeAsync()
    {
        //await _localStack.DebugConsoleAsync(); // Send logs to GitHub Actions Console

        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

        await Localstack.Container.StartAsync(cts.Token).ConfigureAwait(false);

        await OtherInitializeAsync(cts.Token);
    }

    protected abstract IHostBuilder CreateDefaultHostBuilder();

    protected virtual Task OtherInitializeAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await OtherDisposeAsync();

        WireMockServer?.Stop();
        WireMockServer?.Dispose();

        await Localstack.Container.StopAsync();
        await Localstack.Container.DisposeAsync();
    }

    protected virtual Task OtherDisposeAsync() => Task.CompletedTask;
}