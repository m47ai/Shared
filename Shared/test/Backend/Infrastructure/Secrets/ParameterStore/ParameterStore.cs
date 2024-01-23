namespace M47.Shared.Tests.Infrastructure.Secrets.ParameterStore;

using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

public static class ParameterStore
{
    public static async Task<string> GetValueFromParameterStoreAsync(string section, string @namespace)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var name = string.Format("/{0}/{1}/{2}", @namespace, environment, section);
        var request = new GetParameterRequest() { Name = name };

        using var client = new AmazonSimpleSystemsManagementClient();

        var response = await client.GetParameterAsync(request);

        return response.Parameter.Value;
    }

    public static async Task<string> GetValueFromParameterStoreAsync<T>(string section)
    {
        var @namespace = typeof(T).Namespace;
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var name = string.Format("/{0}/{1}/{2}", @namespace, environment, section);

        var request = new GetParameterRequest() { Name = name };

        using var client = new AmazonSimpleSystemsManagementClient();

        var response = await client.GetParameterAsync(request);

        return response.Parameter.Value;
    }
}