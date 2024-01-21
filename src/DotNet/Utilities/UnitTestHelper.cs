using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Core.DotNet.Utilities;

public static class UnitTestHelper
{
    public static TItem Set<TItem>(this IMemoryCache cache, object key, TItem value, MemoryCacheEntryOptions options)
    {
        using (var entry = cache.CreateEntry(key))
        {
            if (options != null)
            {
                entry.SetOptions(options);
            }

            entry.Value = value;
        }

        return value;
    }

    public static IMemoryCache CreateMemoryCache()
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();

        var serviceProvider = services.BuildServiceProvider();

        var memoryCache = serviceProvider.GetService<IMemoryCache>();

        return memoryCache;
    }
}