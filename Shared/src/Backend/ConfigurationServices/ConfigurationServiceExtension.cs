namespace M47.Shared.ConfigurationServices;

using M47.Shared.Domain.Ports;
using M47.Shared.Infrastructure.Decorators;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using System.Net;
using System.Net.Http.Headers;

public static class ConfigurationServiceExtension
{
    public static IConfiguration GetConfiguration(string directory, string projectName, string environment)
    {
        return new ConfigurationBuilder()
            .SetBasePath(directory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddSystemsManager($"/{projectName}/{environment}")
            .AddEnvironmentVariables()
            .Build();
    }

    public static void AddRetryDecorator<Interface, Class>(this IServiceCollection services)
    {
        services.TryAddScoped<RetryDecorator>();

        services.Decorate(typeof(Interface), typeof(Class));
    }

    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int maxRetryCount, Func<int, TimeSpan> sleepDurationProvider)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(maxRetryCount, sleepDurationProvider);
    }

    public static IHttpClientBuilder AddPollyPolicyHandler(this IHttpClientBuilder builder, int maxRetryCount, int secondsBetweenRetries)
        => builder.AddPolicyHandler(PollyRetryPolicy(maxRetryCount, TimeSpan.FromSeconds(secondsBetweenRetries)));

    public static IHttpClientBuilder AddRefreshTokenPolicyHandler<TInterface, TImplementation>(this IHttpClientBuilder builder) where TImplementation : IBearerToken
        => builder.AddPolicyHandler((provider, request) =>
        {
            return Policy.HandleResult<HttpResponseMessage>(r => r.StatusCode is HttpStatusCode.Unauthorized || r.StatusCode is HttpStatusCode.Forbidden)
            .WaitAndRetryAsync(
                retryCount: 1,
                retryAttempt => TimeSpan.FromSeconds(retryAttempt),
                onRetry: (response, timespan) =>
                {
                    var services = provider.GetServices<TInterface>();
                    var service = (IBearerToken)services.First(x => x?.GetType() == typeof(TImplementation))!;

                    if (service is not null)
                    {
                        var task = service.UpdateTokenAsync();
                        task.Wait();

                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", task.Result);
                    }
                });
        });

    public static IAsyncPolicy<HttpResponseMessage> PollyRetryPolicy(int retryCount, TimeSpan medianFirstRetrayDelay)
    {
        return HttpPolicyExtensions.HandleTransientHttpError()
                                   .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
                                   .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(medianFirstRetrayDelay, retryCount));
    }

    public static IHttpClientBuilder AddPollyDecorrelatedJitterBackoffV2Handler<TException, TLogger>(this IHttpClientBuilder builder, int retryCount, TimeSpan medianFirstRetryDelay)
        where TException : Exception
    {
        return builder.AddPolicyHandler((services, request) =>
        {
            return Policy<HttpResponseMessage>
            .Handle<TException>()
            .WaitAndRetryAsync(
                Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(10), retryCount: 20),
                onRetry: async (response, timespan, retryAttempt, context) =>
                {
                    var logger = services.GetService<ILogger<TLogger>>();

                    await SendLogAsync(request, response, timespan, retryAttempt, logger);
                });
        });
    }

    public static IHttpClientBuilder AddPollyDecorrelatedJitterBackoffV2Handler<T>(this IHttpClientBuilder builder, int retryCount, TimeSpan medianFirstRetryDelay)
    {
        return builder.AddPolicyHandler((services, request)
            => HttpPolicyExtensions.HandleTransientHttpError()
                                   .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
                                   .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay, retryCount),
                                                      onRetry: async (response, timespan, retryAttempt, context) =>
                                                      {
                                                          var logger = services.GetService<ILogger<T>>();

                                                          await SendLogAsync(request, response, timespan, retryAttempt, logger);
                                                      }));
    }

    private static async Task SendLogAsync<T>(HttpRequestMessage request, DelegateResult<HttpResponseMessage> response, TimeSpan timespan, int retryAttempt, ILogger<T>? logger)
    {
        var jsonResponse = await response.Result.Content.ReadAsStringAsync();
        var jsonRequest = (request.Content is not null) ? await request.Content.ReadAsStringAsync() : "{}";

        logger?.LogWarning("[HttpError] From middleware received '{httpStatusCode}', delaying for {delay}ms, then making retry {retry}.\n\n- Request:\n{request}.\n\n- Response:\n{response}",
                           response.Result.StatusCode,
                           timespan.TotalMilliseconds,
                           retryAttempt,
                           jsonRequest,
                           jsonResponse);
    }

    public static IHttpClientBuilder AddPollyBackOffPolicyHandler<T>(this IHttpClientBuilder builder, int maxRetryCount, int startWaitSeconds)
    {
        return builder.AddPolicyHandler((services, request) => HttpPolicyExtensions.HandleTransientHttpError()
                      .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
                      .WaitAndRetryAsync(
                            retryCount: maxRetryCount,
                            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) * startWaitSeconds),
                            onRetry: (outcome, timespan, retryAttempt, context) =>
                            {
                                var logger = services.GetService<ILogger<T>>();
                                logger?.LogDebug("Delaying for {delay}ms, then making retry {retry}.", timespan.TotalMilliseconds, retryAttempt);
                            }));
    }

    public static Func<IServiceProvider, HttpRequestMessage, IAsyncPolicy<HttpResponseMessage>> RefreshTokenPolicy<TInterface, TImplementation>()

        where TImplementation : IBearerToken
    {
        return (provider, request) =>
        {
            return Policy.HandleResult<HttpResponseMessage>(r => r.StatusCode is HttpStatusCode.Unauthorized || r.StatusCode is HttpStatusCode.Forbidden)
            .WaitAndRetryAsync(
                retryCount: 1,
                retryAttempt => TimeSpan.FromSeconds(retryAttempt),
                onRetry: (response, timespan) =>
                {
                    var services = provider.GetServices<TInterface>();
                    var service = (IBearerToken)services.First(x => x?.GetType() == typeof(TImplementation))!;

                    if (service is not null)
                    {
                        var task = service.UpdateTokenAsync();
                        task.Wait();

                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", task.Result);
                    }
                });
        };
    }

    public static IHttpClientBuilder AddPollyPolicyHandler<T>(this IHttpClientBuilder builder, int maxRetryCount, int secondsBetweenRetries)
    {
        return builder.AddPolicyHandler((services, request) => HttpPolicyExtensions.HandleTransientHttpError()
                      .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
                      .WaitAndRetryAsync(maxRetryCount, _ => TimeSpan.FromSeconds(secondsBetweenRetries),
                                         onRetry: (outcome, timespan, retryAttempt, context) =>
                                         {
                                             var logger = services.GetService<ILogger<T>>();
                                             logger?.LogDebug("Delaying for {delay}ms, then making retry {retry}.", timespan.TotalMilliseconds, retryAttempt);
                                         }));
    }

    public static IHostBuilder ConfigureParameterStore<T>(this IHostBuilder builder)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (environment is not null)
        {
            if (environment.Contains(Environments.Development))
            {
                environment = Environments.Development;
            }

            var @namespace = typeof(T).Namespace;

            builder.ConfigureAppConfiguration(c =>
            {
                c.AddSystemsManager(source =>
                {
                    source.Path = $"/{@namespace}/{environment}";
                    source.ReloadAfter = TimeSpan.FromHours(1);
                });
            });
        }

        return builder;
    }
}