namespace Core.DotNet.AggregatesModel.ExceptionAggregate;

public abstract class CustomHttpException : Exception
{
    public string Module { get; }
    public string Code { get; }
    public string ErrorMessage { get; }
    public IList<ErrorField> Fields { get; }
    public IList<ErrorRow> Rows { get; }
    public ErrorMessageResponse ErrorMessageResponse { get; }

    private CustomHttpException(string code, string message)
    {
        Code = code;
        ErrorMessage = message;
        Fields = new List<ErrorField>();
        Rows = new List<ErrorRow>();
    }

    public CustomHttpException(string module, string code, string message) : this(code, message)
    {
        Module = module;
    }

    public CustomHttpException(string module, IList<ErrorField> fields, string code = "", string message = "") : this(module, code, message)
    {
        Fields = fields ?? new List<ErrorField>();

        foreach (var field in Fields)
        {
            field.Error = string.IsNullOrWhiteSpace(field.Error) ? string.Empty : $"{field.Error}";
        }
    }

    public CustomHttpException(string module, IList<ErrorRow> rows, string code = "", string message = "") : this(module, code, message)
    {
        Rows = rows ?? new List<ErrorRow>();

        foreach (var row in Rows)
        {
            row.Error = string.IsNullOrWhiteSpace(row.Error) ? string.Empty : $"{row.Error}";
        }
    }

    public CustomHttpException(ErrorMessageResponse errorMessageResponse)
    {
        ErrorMessageResponse = errorMessageResponse;
    }
}