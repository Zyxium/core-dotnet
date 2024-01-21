using Core.DotNet.AggregatesModel.LocationAggregate;
using Core.DotNet.AggregatesModel.TitleAggregate;

namespace Core.DotNet.Domain.Services;

public interface ICacheService
{
    bool TryGetCache<T>(string cacheKey, object request, out T obj);
    void SetCache<T>(string cacheKey, object request, T obj, TimeSpan timeout);
    int ClearCaches();
    int ClearCaches(string cacheKey);

    bool TryGetLocation(LocationCountryCode locationCountryCode, out LocationCountryValue locationCountryValue);
    bool TryGetLocation(LocationCountryValue locationCountrySourceValue, out LocationCountryValue locationCountryValue);
    bool TryGetTitle(TitleCode titleCode, out TitleValue titleValue);
    bool TryGetTitle(TitleValue titleSourceValue, out TitleValue titleValue);
}