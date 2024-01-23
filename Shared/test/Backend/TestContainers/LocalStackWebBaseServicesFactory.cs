namespace M47.Shared.Tests.TestContainers;

using Bogus;
using M47.Shared.ConfigurationServices;
using M47.Shared.Tests.TestContainers.Containers.Localstack;
using M47.Shared.Tests.Utils.Network;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using VerifyTests;
using WireMock.Server;

public abstract class LocalStackWebBaseServicesFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>, IAsyncLifetime
    where TEntryPoint : class
{
    protected readonly WireMockServer WireMockServer;

    public LocalstackContainerBuilder<TEntryPoint> Localstack;

    public LocalStackWebBaseServicesFactory(string group)
    {
        Randomizer.Seed = new Random(666);
        VerifierSettings.ScrubInlineGuids();

        Localstack = new(group);

        WireMockServer = WireMockServer.Start(Ports.GetAvailablePort());
    }

    public T GetService<T>() where T : class
        => Services.GetRequiredService<IServiceScopeFactory>()
                   .CreateScope()
                   .ServiceProvider
                   .GetRequiredService<T>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.Services.AddLogging(config => config.AddConsole());
            logging.SetMinimumLevel(LogLevel.Warning);
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(IHostedService));

            services.ConfigureTestsServicesAws(Localstack.Credentials, Localstack.Uri);
        });
    }

    public async Task InitializeAsync()
    {
        //await _localStack.DebugConsoleAsync(); // Send logs to GitHub Actions Console

        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

        await Localstack.Container.StartAsync(cts.Token).ConfigureAwait(false);

        await OtherInitializeAsync(cts.Token);
    }

    protected virtual Task OtherInitializeAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public new async Task DisposeAsync()
    {
        await OtherDisposeAsync();

        WireMockServer?.Stop();
        WireMockServer?.Dispose();

        await Localstack.Container.StopAsync();
        await Localstack.Container.DisposeAsync();

        await base.DisposeAsync();
    }

    protected virtual Task OtherDisposeAsync() => Task.CompletedTask;
}