using System.Text.Json.Serialization;

namespace Readability.NET;

public class Readability : IReadability
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    public async Task<ReadabilityResult> Parse(string url, ReadabilityOptions? options = null)
    {
        var html = await HttpHelper.GetHtmlString(url);

        var result = await WasmHelper.InvokeModule(html);

        return JsonSerializer.Deserialize<ReadabilityResult>(result, _jsonOptions)!;
    }
}