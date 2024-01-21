using Core.DotNet.AggregatesModel.CommonAggregate;

namespace Core.DotNet.AggregatesModel.ExceptionAggregate;

public class ErrorMessage
{
    public string Domain { get; set; }
    public string Code { get; set; }
    public Locale Label { get; set; }
}