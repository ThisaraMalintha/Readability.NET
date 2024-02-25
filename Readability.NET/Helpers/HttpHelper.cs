namespace Readability.NET.Helpers;

internal class HttpHelper
{
    private static readonly HttpClient _httpClient = new();

    public static async Task<string> GetHtmlString(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            throw new ArgumentException($"'{nameof(url)}' cannot be null or empty.", nameof(url));
        }

        try
        {
            return await _httpClient.GetStringAsync(url).ConfigureAwait(false);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
