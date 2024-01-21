using System.Globalization;
using Core.DotNet.Extensions.Utilities;
using Microsoft.AspNetCore.Http;

namespace Core.DotNet.Middlewares;

public class RequestCultureMiddleware
{
    private readonly RequestDelegate _next;

    public RequestCultureMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext context)
    {
        CultureInfo culture = null;
        string[] appCultures = { "en", "us", "en-us","th", "th-th", "zh-cn", "zh", "cn","ja","jp","ja-jp" };

        var cultureQuery = context.Request.Query["lang"];
        if (!string.IsNullOrWhiteSpace(cultureQuery))
            culture = new CultureInfo(ConvertCultureExtensions.CultureCodeToStandardCultureCode(cultureQuery));

        var cultureHeader = context.Request.Headers["Accept-Language"];
        if (culture == null && !string.IsNullOrWhiteSpace(cultureHeader))
            culture = new CultureInfo(ConvertCultureExtensions.CultureCodeToStandardCultureCode(cultureHeader));

        if (culture is null || !appCultures.ToList().Contains(culture.Name.ToLower()))
            culture = new CultureInfo(ConvertCultureExtensions.CultureCodeToStandardCultureCode("th"));

        Thread.CurrentThread.CurrentUICulture = culture;

        return this._next(context);
    }
}