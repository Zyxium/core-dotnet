using System.Globalization;
using System.Reflection;
using Core.DotNet.AggregatesModel.CommonAggregate;
using Core.DotNet.AggregatesModel.LocationAggregate;
using Core.DotNet.AggregatesModel.ResourceAggregate;
using Core.DotNet.AggregatesModel.TitleAggregate;
using Core.DotNet.Extensions.AggregatesModel.LocationAggregate;
using Core.DotNet.Extensions.AggregatesModel.TitleAggregate;
using Core.DotNet.Utilities.Auth;
using CsvHelper;
using Microsoft.Extensions.Caching.Memory;

namespace Core.DotNet.Domain.Services;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    private static List<(string cacheKey, string cacheKeyUid)> _cacheKeys;

    public CacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;

        if (_cacheKeys is null)
        {
            _cacheKeys = new List<(string cacheKey, string cacheKeyUid)>();
        }
    }

    public bool TryGetCache<T>(string cacheKey, object request, out T obj)
    {
        string cacheKeyUid = GetCacheKey(cacheKey, request);

        // Look for cache key.
        if (!_memoryCache.TryGetValue(cacheKeyUid, out obj))
        {
            return false;
        }

        return true;
    }

    public void SetCache<T>(string cacheKey, object request, T obj, TimeSpan timeout)
    {
        string cacheKeyUid = GetCacheKey(cacheKey, request);

        _memoryCache.Remove(cacheKeyUid);

        _cacheKeys.RemoveAll(m => m.cacheKey == cacheKeyUid);

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(timeout);

        _memoryCache.Set(cacheKeyUid, obj, cacheEntryOptions);

        _cacheKeys.Add((cacheKey, cacheKeyUid));
    }

    private string GetCacheKey(string cacheKey, object request)
    {
        return (request is null) ? cacheKey : $"{cacheKey}_{IdentityHelper.ComputeSha256Hash(request)}";
    }

    public int ClearCaches()
    {
        var result = 0;

        _cacheKeys.ForEach(m =>
        {
            _memoryCache.Remove(m.cacheKey);
            result++;
        });

        _cacheKeys.Clear();

        return result;
    }

    public int ClearCaches(string cacheKey)
    {
        var result = 0;
        var cacheKeys = new List<string>();

        _cacheKeys
            .Where(m => m.cacheKey == cacheKey)
            .ToList()
            .ForEach(m =>
            {
                _memoryCache.Remove(m.cacheKey);
                result++;
                cacheKeys.Add(m.cacheKey);
            });

        cacheKeys.ForEach(m => _cacheKeys.RemoveAll(n => n.cacheKey == m));

        return result;
    }

    public bool TryGetLocation(LocationCountryCode locationCountryCode, out LocationCountryValue locationCountryValue)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "Core.DotNet.DataSources.Location.csv";

        var cacheKey = $"Location_List";

        if (!TryGetCache(cacheKey, null, out List<LocationCountryLocale> locationCache))
        {
            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<LocationResponseMap>();
                locationCache = new List<LocationCountryLocale>();
                locationCache.AddRange(csv.GetRecords<LocationCountryLocale>());
            }

            SetCache(cacheKey, null, locationCache, TimeSpan.FromDays(1));
        }

        var locationLocale = locationCache.FirstOrDefault(m =>
            string.Equals(m.PostalCode.Code, locationCountryCode.PostalCode.Code, StringComparison.InvariantCultureIgnoreCase) &&
            string.Equals(m.District.Code, locationCountryCode.District.Code, StringComparison.InvariantCultureIgnoreCase) &&
            string.Equals(m.SubDistrict.Code, locationCountryCode.SubDistrict.Code, StringComparison.InvariantCultureIgnoreCase) &&
            string.Equals(m.Province.Code, locationCountryCode.Province.Code, StringComparison.InvariantCultureIgnoreCase) &&
            string.Equals(m.Country.Code, locationCountryCode.Country.Code, StringComparison.InvariantCultureIgnoreCase));

        if (locationLocale is null)
        {
            locationCountryValue = null;

            return false;
        }

        locationCountryValue = locationLocale.ToLocationCountryValue();

        return true;
    }

    public bool TryGetLocation(LocationCountryValue locationCountrySourceValue, out LocationCountryValue locationCountryValue)
    {
        return TryGetLocation(new LocationCountryCode
        {
            Country = new ResourceCode
            {
                Code = locationCountrySourceValue.Country.Code
            },
            Province = new ResourceCode
            {
                Code = locationCountrySourceValue.Province.Code
            },
            District = new ResourceCode
            {
                Code = locationCountrySourceValue.District.Code
            },
            SubDistrict = new ResourceCode
            {
                Code = locationCountrySourceValue.SubDistrict.Code
            },
            PostalCode = new ResourceCode
            {
                Code = locationCountrySourceValue.PostalCode.Code
            },
        }, out locationCountryValue);
    }

    public bool TryGetTitle(TitleCode titleCode, out TitleValue titleValue)
    {
        var cacheKey = $"Title_List";

        if (!TryGetCache(cacheKey, null, out List<TitleLocale> titlesCache))
        {
            titlesCache = ReadTitles();

            SetCache(cacheKey, null, titlesCache, TimeSpan.FromDays(1));
        }

        var titleLocale = titlesCache.FirstOrDefault(m => string.Equals(m.Code, titleCode.Code, StringComparison.InvariantCultureIgnoreCase));

        if (titleLocale is null)
        {
            titleValue = null;

            return false;
        }

        titleValue = titleLocale.ToTitleValue();

        return true;
    }

    public bool TryGetTitle(TitleValue titleSourceValue, out TitleValue titleValue)
    {
        return TryGetTitle(new TitleCode { Code = titleSourceValue.Code }, out titleValue);
    }

    public List<TitleLocale> ReadTitles()
    {
        return new List<TitleLocale>
        {
            new TitleLocale
            {
                Code = "NS",
                DisplayName = new Locale
                {
                    EN = "Not specified",
                    TH = "ไม่ระบุ"
                }
            },
            new TitleLocale
            {
                Code = "MR",
                DisplayName = new Locale
                {
                    EN = "Mr.",
                    TH = "นาย"
                }
            },
            new TitleLocale
            {
                Code = "MRS",
                DisplayName = new Locale
                {
                    EN = "Mrs.",
                    TH = "นาง"
                }
            },
            new TitleLocale
            {
                Code = "MISS",
                DisplayName = new Locale
                {
                    EN = "Miss",
                    TH = "นางสาว"
                }
            },
        };
    }
}