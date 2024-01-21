namespace Core.DotNet.AggregatesModel.CommonAggregate;

public class Locale
{
    public string EN { get; set; }
    public string TH { get; set; }

    public override string ToString()
    {
        switch (Thread.CurrentThread.CurrentUICulture.Name.ToLower())
        {
            case LanguageCode.EN:
            case "en-us":
                return EN;
            case LanguageCode.TH:
            case "th-th":
                return TH;
            default:
                return TH;
        }
    }
}

public static class LanguageCode
{
    public const string EN = "en";
    public const string TH = "th";
}