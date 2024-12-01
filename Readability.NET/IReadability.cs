namespace Readability.NET;

public interface IReadability
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="url"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    Task<ReadabilityResult> ParseUrl(string url, ReadabilityOptions? options = null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="html"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    Task<ReadabilityResult> ParseHtml(string html, ReadabilityOptions? options = null);
}