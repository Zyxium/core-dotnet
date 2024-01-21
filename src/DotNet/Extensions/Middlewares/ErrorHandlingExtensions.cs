using Core.DotNet.AggregatesModel.ExceptionAggregate;
using Core.DotNet.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Core.DotNet.Extensions.Middlewares;

public static class ErrorHandlingExtensions
{
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder, ErrorHandlingOptions errorHandlingOptions = null)
    {
        return errorHandlingOptions is null ? builder.UseMiddleware(typeof(ErrorHandlingMiddleware)) :
            builder.UseMiddleware(typeof(ErrorHandlingMiddleware), errorHandlingOptions);
    }
}