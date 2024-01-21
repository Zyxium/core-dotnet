using System.Globalization;
using System.Runtime.InteropServices;

namespace Core.DotNet.Extensions.Utilities;

public static class DateTimeExtensions
{
    public static long ToUnixTime(this DateTime dateTime)
    {
        DateTime dateTime1 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return (long)(dateTime - dateTime1).TotalSeconds;
    }

    public static void SetCultureInfo(string culture)
    {
        CultureInfo enCulture = new CultureInfo(culture);
        
        // Set current thread's culture
        System.Threading.Thread.CurrentThread.CurrentCulture = enCulture;
        System.Threading.Thread.CurrentThread.CurrentUICulture = enCulture;
    }

    public static DateTime ToDateTime(this long unixTime) => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds((double)unixTime);

    public static DateTime ToThaiDateTime(this DateTime dateTime) => dateTime.GmtToPacific(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "SE Asia Standard Time" : "Asia/Bangkok");

    public static DateTime GmtToPacific(this DateTime dateTime, string timeZoneId) => TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));
}