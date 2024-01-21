namespace Core.DotNet.Extensions.Utilities;

public static class EnumExtensions
{
    public static string GetName<T>(this T enumTarget) where T : System.Enum => enumTarget.ToString();

    public static T GetEnum<T>(this string enumValue) where T : System.Enum => (T)
        System.Enum.Parse(typeof(T), enumValue, ignoreCase: true);
}