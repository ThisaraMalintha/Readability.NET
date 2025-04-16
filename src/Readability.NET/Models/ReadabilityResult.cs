namespace Readability.NET.Models;

public class ReadabilityResult
{
    public string? Title { get; set; }
    public string? Byline { get; set; }
    public HtmlContentDirection? Dir { get; set; } = HtmlContentDirection.ltr;
    public string? Lang { get; set; }
    public string? Content { get; set; }
    public string? TextContent { get; set; }
    public int Length { get; set; }
    public string? Excerpt { get; set; }
    public string? SiteName { get; set; }

    [JsonConverter(typeof(StringToDateTimeJsonConverter))]
    public DateTimeOffset? PublishedTime { get; set; }
}

public enum HtmlContentDirection
{
    ltr,
    rtl
}