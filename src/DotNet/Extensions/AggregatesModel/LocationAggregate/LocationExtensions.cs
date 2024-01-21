using Core.DotNet.AggregatesModel.LocationAggregate;
using Core.DotNet.AggregatesModel.ResourceAggregate;
using Core.DotNet.Domain.Services;
using Core.DotNet.Utilities;

namespace Core.DotNet.Extensions.AggregatesModel.LocationAggregate;

public static class LocationExtensions
{
    public static LocationCountryValue ToLocationCountryValue(this LocationCountryLocale locationCountryLocale)
    {
        return new LocationCountryValue
        {
            Country = new ResourceValue
            {
                Code = locationCountryLocale.Country.Code,
                DisplayName = locationCountryLocale.Country.DisplayName.ToString()
            },
            Province = new ResourceValue
            {
                Code = locationCountryLocale.Province.Code,
                DisplayName = locationCountryLocale.Province.DisplayName.ToString()
            },
            District = new ResourceValue
            {
                Code = locationCountryLocale.District.Code,
                DisplayName = locationCountryLocale.District.DisplayName.ToString()
            },
            SubDistrict = new ResourceValue
            {
                Code = locationCountryLocale.SubDistrict.Code,
                DisplayName = locationCountryLocale.SubDistrict.DisplayName.ToString()
            },
            PostalCode = new ResourceValue
            {
                Code = locationCountryLocale.PostalCode.Code,
                DisplayName = locationCountryLocale.PostalCode.DisplayName.ToString()
            }
        };
    }

    public static string GetLocationDisplay(this Location location)
    {
        string result;

        if (RequestHelper.IsLanguageThai())
        {
            result = (string.IsNullOrEmpty(location.Address) ? "" : " " + location.Address) +
                     (string.IsNullOrEmpty(location.SubDistrict?.Code) ? "" : " ตำบล/แขวง " + location.SubDistrict.DisplayName) +
                     (string.IsNullOrEmpty(location.District?.Code) ? "" : " อำเภอ/เขต " + location.District.DisplayName) +
                     (string.IsNullOrEmpty(location.Province?.Code) ? "" : " จังหวัด " + location.Province.DisplayName) +
                     (string.IsNullOrEmpty(location.PostalCode?.Code) ? "" : " รหัสไปรษณีย์ " + location.PostalCode.DisplayName);
        }
        else
        {
            result = (string.IsNullOrEmpty(location.Address) ? "" : location.Address) +
                     (string.IsNullOrEmpty(location.SubDistrict?.Code) ? "" : ", " + location.SubDistrict.DisplayName) +
                     (string.IsNullOrEmpty(location.District?.Code) ? "" : ", " + location.District.DisplayName) +
                     (string.IsNullOrEmpty(location.Province?.Code) ? "" : ", " + location.Province.DisplayName) +
                     (string.IsNullOrEmpty(location.PostalCode?.Code) ? "" : " " + location.PostalCode.DisplayName);
        }

        return result;
    }

    public static string GetLocationDisplay(this Location location, ICacheService cacheService)
    {
        _ = cacheService.TryGetLocation(new LocationCountryCode
        {
            Country = new ResourceCode
            {
                Code = location.Country.Code
            },
            Province = new ResourceCode
            {
                Code = location.Province.Code
            },
            District = new ResourceCode
            {
                Code = location.District.Code
            },
            SubDistrict = new ResourceCode
            {
                Code = location.SubDistrict.Code
            },
            PostalCode = new ResourceCode
            {
                Code = location.PostalCode.Code
            }
        }, out var locationCountry);

        location.SubDistrict = locationCountry.SubDistrict;
        location.District = locationCountry.District;
        location.Province = locationCountry.Province;
        location.PostalCode = locationCountry.PostalCode;

        return GetLocationDisplay(location);
    }

    public static string GetLocationDisplayDocument(this Location location)
    {
        string result;

        if (RequestHelper.IsLanguageThai())
        {
            // TH-10 = "กรุงเทพมหนคร"
            if (location.Province.Code == "TH-10")
            {
                result = (string.IsNullOrEmpty(location.Address) ? "" : " " + location.Address) +
                         (string.IsNullOrEmpty(location.SubDistrict?.Code) ? "" : " แขวง " + location.SubDistrict.DisplayName) +
                         (string.IsNullOrEmpty(location.District?.Code) ? "" : " เขต " + location.District.DisplayName) +
                         (string.IsNullOrEmpty(location.Province?.Code) ? "" : " " + location.Province.DisplayName) +
                         (string.IsNullOrEmpty(location.PostalCode?.Code) ? "" : " " +  location.PostalCode.DisplayName);
            }
            else
            {
                result = (string.IsNullOrEmpty(location.Address) ? "" : " " + location.Address) +
                         (string.IsNullOrEmpty(location.SubDistrict?.Code) ? "" : " ตำบล " + location.SubDistrict.DisplayName) +
                         (string.IsNullOrEmpty(location.District?.Code) ? "" : " อำเภอ " + location.District.DisplayName) +
                         (string.IsNullOrEmpty(location.Province?.Code) ? "" : " " + location.Province.DisplayName) +
                         (string.IsNullOrEmpty(location.PostalCode?.Code) ? "" : " " + location.PostalCode.DisplayName);
            }
        }
        else
        {
            result = (string.IsNullOrEmpty(location.Address) ? "" : location.Address) +
                     (string.IsNullOrEmpty(location.SubDistrict?.Code) ? "" : ", " + location.SubDistrict.DisplayName) +
                     (string.IsNullOrEmpty(location.District?.Code) ? "" : ", " + location.District.DisplayName) +
                     (string.IsNullOrEmpty(location.Province?.Code) ? "" : ", " + location.Province.DisplayName) +
                     (string.IsNullOrEmpty(location.PostalCode?.Code) ? "" : " " + location.PostalCode.DisplayName);
        }

        return result;
    }
}