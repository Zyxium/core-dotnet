using Core.DotNet.AggregatesModel.HttpAggregate;
using Core.DotNet.AggregatesModel.TelemetryInitializerAggregate;
using Core.DotNet.Utilities.Hosting;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.DotNet.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomAppInsightsService(this IServiceCollection services, IConfiguration configuration, string version)
    {
        return services.AddCustomAppInsightsService(
            configuration["APPINSIGHTS_ROLE_NAME"],
            configuration["APPINSIGHTS_INSTRUMENTATION_KEY"],
            version);
    }

    public static IServiceCollection AddCustomAppInsightsService(this IServiceCollection services, string roleName, string instrumentationKey, string version)
    {
        if (EnvironmentHelper.IsLocal ||
            EnvironmentHelper.IsDevelopment ||
            EnvironmentHelper.IsUAT ||
            EnvironmentHelper.IsProduction)
        {
            services.AddSingleton<ITelemetryInitializer>(m => new TelemetryInitializer(roleName, m.GetService<IHttpContextAccessor>())
            {
                RequestHeaders = new List<string>
                {
                    StandardHeader.AcceptLanguage,
                    StandardHeader.CorrelationContext,
                    StandardHeader.RequestId,
                    StandardHeader.XRequestId,
                    StandardHeader.XRealIP,
                    StandardHeader.XPlatform,
                    StandardHeader.XApplication,
                    StandardHeader.XTraceId,
                    StandardHeader.XDeviceId
                }
            });

            services.AddSingleton<ITelemetryInitializer, CloneIPAddress>();

            var applicationInsightsServiceOptions = new ApplicationInsightsServiceOptions
            {
                InstrumentationKey = instrumentationKey,
                ApplicationVersion = version,
                DeveloperMode = EnvironmentHelper.IsLocal,
            };

            services.AddApplicationInsightsTelemetry(applicationInsightsServiceOptions);
            services.AddApplicationInsightsKubernetesEnricher();
        }

        return services;
    }
}