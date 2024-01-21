using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Core.DotNet.Extensions.DependencyInjection;

public static class ApplicationBuilderExtensions
{
    public static void UpdateDatabase<T>(this IApplicationBuilder app) where T : Microsoft.EntityFrameworkCore.DbContext
    {
        using var serviceScope = app.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope();

        using var context = serviceScope.ServiceProvider.GetService<T>();

        context?.Database.Migrate();
    }
}