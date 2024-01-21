namespace Core.DotNet.AggregatesModel.ExceptionAggregate;

public class ErrorHandlingOptions
{
    public string ServiceName { get; }

    public ErrorHandlingOptions(string serviceName)
    {
        ServiceName = serviceName;
    }
}