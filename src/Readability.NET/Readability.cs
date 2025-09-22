namespace Readability.NET;

public class Readability : IReadability, IDisposable
{
    private readonly Lazy<HttpClient> _httpClient = new(LazyThreadSafetyMode.ExecutionAndPublication);
    private readonly IReadabilityWasmModule _readabilityWasmModule = new ReadabilityWasmModule();

    public async Task<ReadabilityResult> ParseUrl(string url, ReadabilityOptions? options = default)
    {
        var html = await _httpClient.Value.GetHtmlString(url);

        return await ParseHtml(html, options);
    }

    public async Task<ReadabilityResult> ParseHtml(string html, ReadabilityOptions? options = default)
    {
        return await _readabilityWasmModule.Invoke(html, options);
    }

    public void Dispose()
    {
        _readabilityWasmModule.Dispose();

        if (_httpClient.IsValueCreated)
        {
            _httpClient.Value?.Dispose();
        }
    }
}