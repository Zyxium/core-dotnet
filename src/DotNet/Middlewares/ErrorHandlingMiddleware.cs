using Core.DotNet.AggregatesModel.ExceptionAggregate;
using Core.DotNet.Extensions.Middlewares;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Security.Authentication;
using System.Text.Json;

namespace Core.DotNet.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ErrorHandlingOptions _errorHandlingOptions;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public ErrorHandlingMiddleware(RequestDelegate next, ErrorHandlingOptions errorHandlingOptions) : this(next)
    {
        _errorHandlingOptions = errorHandlingOptions;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(_errorHandlingOptions, context, ex);
        }
    }

    private static Task HandleExceptionAsync(ErrorHandlingOptions errorHandlingOptions, HttpContext context, Exception exception)
    {
        // TODO: [LOG ERROR]

        var code = HttpStatusCode.InternalServerError;

        switch (exception)
        {
            case ArgumentNullException:
            case ArgumentException:
            case HttpRequestException:
            case NotSupportedException:
                code = HttpStatusCode.BadRequest;
                break;
            case AuthenticationException:
                code = HttpStatusCode.Unauthorized;
                break;
            case UnauthorizedAccessException:
                code = HttpStatusCode.Forbidden;
                break;
            case KeyNotFoundException:
                code = HttpStatusCode.NotFound;
                break;
            case CustomHttpException customHttpException:
                code = customHttpException is CustomHttpNotFoundException
                    ? HttpStatusCode.NotFound
                    : HttpStatusCode.BadRequest;
                return WriteExceptionAsync(errorHandlingOptions,
                    context,
                    exception,
                    code,
                    errorHandlingOptions.ServiceName,
                    customHttpException.Module,
                    customHttpException.Code,
                    customHttpException.ErrorMessage,
                    customHttpException.Fields,
                    customHttpException.Rows,
                    customHttpException.ErrorMessageResponse);
        }

        return WriteExceptionAsync(errorHandlingOptions, context, exception, code);
    }

    private static Task WriteExceptionAsync(ErrorHandlingOptions errorHandlingOptions, HttpContext context, Exception exception, HttpStatusCode httpStatusCode, string domain = "", string module = "", string code = "", string message = "", IList<ErrorField> fields = null, IList<ErrorRow> rows = null, ErrorMessageResponse errorMessageResponse = null)
    {
        var response = context.Response;
        response.StatusCode = (int)httpStatusCode;
        response.ContentType = "application/json";

        switch (httpStatusCode)
        {
            case HttpStatusCode.Unauthorized:
                return response.WriteAsync(CreateErrorMessageResponse(errorHandlingOptions.ServiceName, "unauthorized", "Your session has expired. Please log out and try to login again.", fields, rows));
            case HttpStatusCode.Forbidden:
                return response.WriteAsync(CreateErrorMessageResponse(errorHandlingOptions.ServiceName, "forbidden", "Access to this resource on the server is denied!", fields, rows));
            case HttpStatusCode.InternalServerError:
                return response.WriteAsync(CreateErrorMessageResponse(errorHandlingOptions.ServiceName, "internal server error", "Something went wrong. Please reload this page again.", fields, rows));
        }

        //Forward error message response
        if (errorMessageResponse != null)
        {
            return response.WriteAsync(JsonSerializer.Serialize(errorMessageResponse,
                new JsonSerializerOptions
                {
                    IgnoreNullValues = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));
        }

        if (string.IsNullOrEmpty(code))
        {
            code = httpStatusCode.ToStandardCode();
        }

        return response.WriteAsync(CreateErrorMessageResponse(domain, $"{module}_{code}".ToLower(),
            string.IsNullOrEmpty(message) ? httpStatusCode.ToStandardCode() : message,
            fields, rows));
    }

    private static string CreateErrorMessageResponse(string domain, string code, string message,
        IList<ErrorField> fields, IList<ErrorRow> rows)
    {
        var errorResponse = new ErrorMessageResponse()
        {
            Domain = domain,
            Code = code,
            Message = message
        };

        if (fields is { Count: > 0 })
        {
            errorResponse.Fields = fields;
        }

        if (rows is { Count: > 0 })
        {
            errorResponse.Rows = rows;
        }

        return JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
}