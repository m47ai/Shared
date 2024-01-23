namespace M47.Shared.Tests
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Polly;
    using Polly.Extensions.Http;
    using System;
    using System.IO;
    using System.Net.Http;

    public abstract class InjectionBaseTest : BaseTest
    {
        private readonly IServiceProvider _serviceProvider;

        public InjectionBaseTest()
        {
            _serviceProvider = ConfigureServiceProvider().BuildServiceProvider();
        }

        protected abstract IServiceCollection ConfigureServiceProvider();

        protected T GetFromServices<T>() => _serviceProvider.GetService<T>()!;

        protected static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        protected static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}