namespace M47.Shared.Extensions;

using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Linq;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection Remove<T>(this IServiceCollection services)
    {
        if (services.IsReadOnly)
        {
            throw new ReadOnlyException($"{nameof(services)} is read only");
        }

        var serviceDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(T));
        if (serviceDescriptor is not null)
        {
            services.Remove(serviceDescriptor);
        }

        return services;
    }

    public static void Replace<TService>(this IServiceCollection services, TService implementation) where TService : class
    {
        var descriptor = new ServiceDescriptor(typeof(TService), implementation);

        services.Replace(descriptor);
    }
}