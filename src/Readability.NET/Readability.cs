namespace Readability.NET;

public class Readability : IReadability
{
    private readonly HttpClient? _httpClient;
    private readonly IHttpClientFactory? _httpClientFactory;
    private readonly IReadabilityWasmModule _readabilityWasmModule = new ReadabilityWasmModule();

    public Readability(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public Readability()
    {
        _httpClient = new HttpClient();
    }

    public async Task<ReadabilityResult> ParseUrl(string url, ReadabilityOptions? options = default)
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

    public async Task<ReadabilityResult> ParseHtml(string html, ReadabilityOptions? options = default)
    {
        return await _readabilityWasmModule.Invoke(html, options);
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