namespace Readability.NET;

public class Readability : IReadability
{
    private readonly HttpClient? _httpClient;
    private readonly IHttpClientFactory? _httpClientFactory;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    public Readability(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public Readability(HttpClient? httpClient = default)
    {
        _httpClient = httpClient ?? new HttpClient();
    }

    public async Task<ReadabilityResult> ParseUrl(string url, ReadabilityOptions? options = null)
    {
        try
        {
            var httpClient = GetHttpClient();

            var html = await httpClient.GetHtmlString(url);

            return await ParseHtml(html, options);
        }
        finally
        {
            // Dispose the IHttpClientFactory provided http clients.
            if (_httpClientFactory != null)
            {
                _httpClient?.Dispose();
            }
        }
    }

    public async Task<ReadabilityResult> ParseHtml(string html, ReadabilityOptions? options = null)
    {
        var result = await WasmHelper.InvokeModule(html);

        return JsonSerializer.Deserialize<ReadabilityResult>(result, _jsonOptions)!;
    }

    private HttpClient GetHttpClient()
    {
        return (_httpClientFactory, _httpClient) switch
        {
            (not null, _) => _httpClientFactory.CreateClient(),
            (_, not null) => _httpClient,
            (null, null) => throw new InvalidOperationException("Failed to create a http client instance."),
        };
    }
}