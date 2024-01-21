using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.DotNet.Extensions.Utilities;

public static class JsonSerializerExtensions
{
    public static T DeserializerObject<T>(this string json) where T : new()
    {
        var result = JsonSerializer.Deserialize<T>(json,
            new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

        return result;
    }
}