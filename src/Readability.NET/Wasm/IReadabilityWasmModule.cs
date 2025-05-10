namespace Readability.NET.Wasm;

public interface IReadabilityWasmModule : IDisposable
{
    Task<ReadabilityResult> Invoke(string html, ReadabilityOptions? options = default);
}
