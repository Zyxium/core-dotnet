namespace Core.DotNet.AggregatesModel.ExceptionAggregate;

public class ErrorField
{
    public string Field { get; set; }
    public string Error { get; set; }
    
    public ErrorField() { }

    public ErrorField(string field)
    {
        Field = field;
    }

    public ErrorField(string field, string error) : this(field)
    {
        Error = error;
    }
}