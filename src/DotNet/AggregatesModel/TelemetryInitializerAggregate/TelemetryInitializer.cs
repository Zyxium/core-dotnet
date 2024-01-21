using Core.DotNet.Utilities.Hosting;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;

namespace Core.DotNet.AggregatesModel.TelemetryInitializerAggregate;

public class TelemetryInitializer : ITelemetryInitializer
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly string _roleName;

    public List<string> RequestHeaders { get; set; }
    public List<string> ResponseHeaders { get; set; }

    /// <summary>
    /// Create Telemetry Initializer
    /// </summary>
    public TelemetryInitializer() : base()
    {
    }

    /// <summary>
    /// Create Telemetry Initializer
    /// </summary>
    /// <param name="roleName">Role name or application name</param>
    public TelemetryInitializer(string roleName) : this()
    {
        _roleName = roleName;
    }

    /// <summary>
    /// Create Telemetry Initializer 
    /// </summary>
    /// <param name="roleName"></param>
    /// <param name="httpContextAccessor"></param>
    public TelemetryInitializer(string roleName, IHttpContextAccessor httpContextAccessor) : this(roleName)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Pass values to Application Insights
    /// </summary>
    /// <param name="telemetry"></param>
    public void Initialize(ITelemetry telemetry)
    {
        telemetry.Context.Cloud.RoleName = _roleName;
        telemetry.Context.GlobalProperties["DeveloperMode"] = EnvironmentHelper.IsLocal ? "true" : "false";

        #region Track Request
        if (_httpContextAccessor is not null &&
            _httpContextAccessor.HttpContext is not null)
        {
            if (RequestHeaders != null)
            {
                foreach (var headerName in RequestHeaders)
                {
                    if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey(headerName))
                        telemetry.Context.GlobalProperties[$"Request {headerName}"] = _httpContextAccessor.HttpContext.Request.Headers[headerName];
                }
            }

            if (ResponseHeaders != null)
            {
                foreach (var headerName in ResponseHeaders)
                {
                    if (_httpContextAccessor.HttpContext.Response.Headers.ContainsKey(headerName))
                        telemetry.Context.GlobalProperties[$"Response {headerName}"] = _httpContextAccessor.HttpContext.Response.Headers[headerName];
                }
            }
        }
        #endregion
    }
}