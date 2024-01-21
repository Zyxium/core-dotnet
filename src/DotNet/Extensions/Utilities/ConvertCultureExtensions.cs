namespace Core.DotNet.Extensions.Utilities;

public static class ConvertCultureExtensions
{
    public static string CultureCodeToStandardCultureCode(string letter)
    {
        letter = letter.Trim().ToLower();

        switch (letter)
        {
            case "en":
            case "en-us":
                return "en-US";
            case "th":
            case "th-th":
                return "th-TH";
            default:
                return "th-TH";
        }
    }
}