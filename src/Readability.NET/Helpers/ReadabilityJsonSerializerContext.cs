namespace Readability.NET;

[JsonSerializable(typeof(ReadabilityWasmModule.ReadabilityWasmInput))]
[JsonSerializable(typeof(ReadabilityResult))]
[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    UseStringEnumConverter = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase
)]
internal partial class ReadabilityJsonSerializerContext : JsonSerializerContext
{
}
