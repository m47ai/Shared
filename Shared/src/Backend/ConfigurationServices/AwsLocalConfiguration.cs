namespace M47.Shared.ConfigurationServices;

using Amazon.Runtime;
using Amazon.S3;
using Amazon.SimpleEmail;
using Amazon.SQS;
using M47.Shared.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public static class AwsLocalConfiguration
{
    public static void ConfigureAwsLocalDevelopment(this IServiceCollection services, IConfiguration configuration)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (environment == Environments.Production)
        {
            return;
        }

        var serviceUrl = configuration.GetSection("Localstack:ServiceUrl").Value!;
        var credentials = new BasicAWSCredentials("test", "test");

        if (environment is null)
        {
            services.ConfigureTestsServicesAws(credentials, serviceUrl);
            return;
        }

        services.ConfigureLocalServicesAws(credentials, serviceUrl);
    }

    public static void ConfigureLocalServicesAws(this IServiceCollection services, AWSCredentials credentials,
                                                 string serviceUrl)
    {
        services.Remove<IAmazonS3>();
        services.AddSingleton(CreateS3Client(credentials, serviceUrl));

        services.Remove<IAmazonSQS>();
        services.AddSingleton(CreateSqsClient(credentials, serviceUrl));
    }

    public static void ConfigureTestsServicesAws(this IServiceCollection services, AWSCredentials credentials,
                                                 string serviceUrl)
    {
        services.ConfigureLocalServicesAws(credentials, serviceUrl);

        services.Remove<IAmazonSimpleEmailService>();
        services.AddSingleton(CreateSesClient(credentials, serviceUrl));
    }

    public static IAmazonS3 CreateS3Client(AWSCredentials credentials, string serviceUrl)
        => new AmazonS3Client(credentials, new AmazonS3Config()
        {
            ServiceURL = serviceUrl,
            ForcePathStyle = true,
        });

    public static IAmazonSQS CreateSqsClient(AWSCredentials credentials, string serviceUrl)
        => new AmazonSQSClient(credentials, new AmazonSQSConfig()
        {
            ServiceURL = serviceUrl
        });

    public static IAmazonSimpleEmailService CreateSesClient(AWSCredentials credentials, string serviceUrl)
        => new AmazonSimpleEmailServiceClient(credentials, new AmazonSimpleEmailServiceConfig()
        {
            ServiceURL = serviceUrl,
        });
}