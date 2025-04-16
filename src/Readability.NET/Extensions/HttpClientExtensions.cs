using System.Net.Mime;

namespace Readability.NET.Extensions;

internal static class HttpClientExtensions
{
    public static async Task<string> GetHtmlString(this HttpClient httpClient, string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            throw new ArgumentException($"'{nameof(url)}' cannot be null or empty.", nameof(url));
        }

        try
        {
            var response = await httpClient.GetAsync(url).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Request failed. Status:{response.StatusCode}");
            }

            if (response.Content.Headers.ContentType.MediaType != MediaTypeNames.Text.Html)
            {
                throw new Exception("No html content found");
            }

            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
