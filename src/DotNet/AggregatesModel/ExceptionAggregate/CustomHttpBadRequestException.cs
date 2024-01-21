namespace Core.DotNet.AggregatesModel.ExceptionAggregate;

public class CustomHttpBadRequestException : CustomHttpException
{
    public CustomHttpBadRequestException(string module, string code, string message) : base(module, code, message)
    {
    }

    public CustomHttpBadRequestException(string module, IList<ErrorField> fields, string code = "", string message = "") 
        : base(module, fields, code, message)
    {
    }

    public CustomHttpBadRequestException(string module, IList<ErrorRow> rows, string code = "", string message = "") 
        : base(module, rows, code, message)
    {
    }

    public CustomHttpBadRequestException(ErrorMessageResponse errorMessageResponse) : base(errorMessageResponse)
    {
    }
}