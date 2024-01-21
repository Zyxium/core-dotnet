using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.DotNet.Extensions.DependencyInjection;

public static class ServiceOptionsExtension
{
    public static T AddCustomOptions<T>(this IServiceCollection services, IConfiguration configuration) where T : class
    {
        services.AddOptions<T>()
            .Bind(configuration)
            .ValidateDataAnnotations();

        var options = configuration.Get<T>();

        return options;
    }
}