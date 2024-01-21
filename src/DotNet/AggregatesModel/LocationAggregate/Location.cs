using CsvHelper.Configuration;
using Core.DotNet.AggregatesModel.CommonAggregate;
using Core.DotNet.AggregatesModel.ResourceAggregate;

namespace Core.DotNet.AggregatesModel.LocationAggregate;

public class LocationDataValueLocale
{
    public string Code { get; set; }
    public Locale DisplayName { get; set; }
}

public class LocationCountryLocale
{
    public LocationDataValueLocale Country { get; set; }
    public LocationDataValueLocale Province { get; set; }
    public LocationDataValueLocale District { get; set; }
    public LocationDataValueLocale SubDistrict { get; set; }
    public LocationDataValueLocale PostalCode { get; set; }
}

public class LocationCountryCode
{
    public ResourceCode Country { get; set; }
    public ResourceCode Province { get; set; }
    public ResourceCode District { get; set; }
    public ResourceCode SubDistrict { get; set; }
    public ResourceCode PostalCode { get; set; }
}

public class LocationCountryValue
{
    public ResourceValue Country { get; set; }
    public ResourceValue Province { get; set; }
    public ResourceValue District { get; set; }
    public ResourceValue SubDistrict { get; set; }
    public ResourceValue PostalCode { get; set; }
}

public sealed class LocationResponseMap : ClassMap<LocationCountryLocale>
{
    public LocationResponseMap()
    {
        Map(m => m.Country.Code).Name("Country/Code");
        Map(m => m.Country.DisplayName.EN).Name("Country/DisplayName/EN");
        Map(m => m.Country.DisplayName.TH).Name("Country/DisplayName/TH");

        Map(m => m.Province.Code).Name("Province/Code");
        Map(m => m.Province.DisplayName.EN).Name("Province/DisplayName/EN");
        Map(m => m.Province.DisplayName.TH).Name("Province/DisplayName/TH");

        Map(m => m.District.Code).Name("District/Code");
        Map(m => m.District.DisplayName.EN).Name("District/DisplayName/EN");
        Map(m => m.District.DisplayName.TH).Name("District/DisplayName/TH");

        Map(m => m.SubDistrict.Code).Name("Subdistrict/Code");
        Map(m => m.SubDistrict.DisplayName.EN).Name("Subdistrict/DisplayName/EN");
        Map(m => m.SubDistrict.DisplayName.TH).Name("Subdistrict/DisplayName/TH");

        Map(m => m.PostalCode.Code).Name("PostalCode/Code");
        Map(m => m.PostalCode.DisplayName.EN).Name("PostalCode/DisplayName/EN");
        Map(m => m.PostalCode.DisplayName.TH).Name("PostalCode/DisplayName/TH");
    }
}

public class Location : LocationCountryValue
{
    public string Type { get; set; }
    public string Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string Description { get; set; }

    public void SetLocationCountry(LocationCountryValue locationCountryValue)
    {
        Country = locationCountryValue.Country;
        Province = locationCountryValue.Province;
        District = locationCountryValue.District;
        SubDistrict = locationCountryValue.SubDistrict;
        PostalCode = locationCountryValue.PostalCode;
    }
}