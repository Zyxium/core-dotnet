using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Core.DotNet.AggregatesModel.TelemetryInitializerAggregate;

public class CloneIPAddress : ITelemetryInitializer
{
    public void Initialize(ITelemetry telemetry)
    {
        var propTelemetry = telemetry as ISupportProperties;
        if (propTelemetry != null && !propTelemetry.Properties.ContainsKey("client-ip"))
        {
            string clientIPValue = telemetry.Context.Location.Ip;
            propTelemetry.Properties.Add("client-ip", clientIPValue);
        }
    }
}