namespace Readability.NET.Wasm;

public interface IReadabilityWasmModule
{
    Task<ReadabilityResult> Invoke(string html, ReadabilityOptions? options = default);
}
