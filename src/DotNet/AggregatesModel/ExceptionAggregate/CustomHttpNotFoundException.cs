namespace Core.DotNet.AggregatesModel.ExceptionAggregate;

public class CustomHttpNotFoundException : CustomHttpException
{
    public CustomHttpNotFoundException(string module, string code, string message) : base(module, code, message)
    {
    }

    public CustomHttpNotFoundException(string module, IList<ErrorField> fields, string code = "", string message = "") 
        : base(module, fields, code, message)
    {
    }

    public CustomHttpNotFoundException(string module, IList<ErrorRow> rows, string code = "", string message = "") 
        : base(module, rows, code, message)
    {
    }

    public CustomHttpNotFoundException(ErrorMessageResponse errorMessageResponse) : base(errorMessageResponse)
    {
    }
}