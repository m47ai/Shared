namespace M47.Shared.Tests.TestContainers;

using Bogus;
using M47.Shared.Tests.Utils.Network;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using VerifyTests;
using WireMock.Server;

public abstract class WebBaseServicesFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>, IAsyncLifetime
    where TEntryPoint : class
{
    protected readonly WireMockServer WireMockServer;
    protected readonly IConfiguration Configuration;

    public WebBaseServicesFactory()
    {
        Randomizer.Seed = new Random(666);
        VerifierSettings.ScrubInlineGuids();

        Configuration = new ConfigurationBuilder()
                            .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"))
                            .Build();
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
        });
    }

    public async Task InitializeAsync()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

        await OtherInitializeAsync(cts.Token);
    }

    protected virtual Task OtherInitializeAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public new async Task DisposeAsync()
    {
        await OtherDisposeAsync();

        WireMockServer?.Stop();
        WireMockServer?.Dispose();

        await base.DisposeAsync();
    }

    protected virtual Task OtherDisposeAsync() => Task.CompletedTask;
}