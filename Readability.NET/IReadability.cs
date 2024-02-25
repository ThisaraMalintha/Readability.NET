namespace Readability.NET;

public interface IReadability
{
    Task<ReadabilityResult> Parse(string url, ReadabilityOptions? options = null);
}