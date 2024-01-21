using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.DotNet.AggregatesModel.ExceptionAggregate;

public class ErrorMessageResponse
{
    public string Domain { get; set; }
    public string Code { get; set; }
    public string Message { get; set; }
    public IList<ErrorField> Fields { get; set; }
    public IList<ErrorRow> Rows { get; set; }

    public ErrorMessageResponse()
    {
        Fields = new List<ErrorField>();
        Rows = new List<ErrorRow>();
    }

    public string ToJson(bool ignoreNullValue)
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = ignoreNullValue ? JsonIgnoreCondition.WhenWritingNull : default,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });
    }
}