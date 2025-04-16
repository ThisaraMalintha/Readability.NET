using Wasmtime;

namespace Readability.NET.Wasm;

public class ReadabilityWasmModule : IReadabilityWasmModule
{
    private const string WasmFileResourceKey = "Readability.NET.Wasm.lib.dist.mozilla-readability.wasm";

    private static readonly Engine _engine = new();

    public async Task<ReadabilityResult> Invoke(string html, ReadabilityOptions? options = default)
    {
        var instanceId = Guid.NewGuid().ToString();

        var stdIn = $"{instanceId}_stdin.f";
        var stdOut = $"{instanceId}_stdout.f";
        var stdError = $"{instanceId}_stderr.f";

        await WriteReadabilityWasmInputToStdIn(stdIn, html, options)
            .ConfigureAwait(false);

        try
        {
            await Task.Run(() => InvokeJavyWasmFunction(stdIn, stdOut, stdError))
                .ConfigureAwait(false);

            using var streamReader = new StreamReader($"{instanceId}_stdout.f");

            var result = await streamReader.ReadToEndAsync();

            return JsonSerializer.Deserialize(result, ReadabilityJsonSerializerContext.Default.ReadabilityResult);
        }
        catch (Exception ex)
        {
            var wasmError = string.Empty;

            if (File.Exists(stdError))
            {
                using var errReader = new StreamReader(stdError, Encoding.UTF8);
                wasmError = string.Join(Environment.NewLine, File.ReadLines(stdError).Take(1000));
            }

            throw new ReadabilityException(wasmError, ex);
        }
        finally
        {
            File.Delete(stdIn);
            File.Delete(stdOut);
            File.Delete(stdError);
        }
    }

    private static void InvokeJavyWasmFunction(string stdIn, string stdOut, string stdError)
    {
        using var wasmModuleStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(WasmFileResourceKey);
        using var store = new Store(_engine);
        using var linker = new Linker(_engine);
        using var module = Wasmtime.Module.FromStream(_engine, WasmFileResourceKey, wasmModuleStream);

        var wasiConfig = new WasiConfiguration()
            .WithStandardInput(stdIn)
            .WithStandardOutput(stdOut)
            .WithStandardError(stdError);

        store.SetWasiConfiguration(wasiConfig);
        linker.DefineWasi();

        var instance = linker.Instantiate(store, module);
        var javyStart = instance.GetAction("_start")!; // Javy default startup function

        javyStart();
    }

    private static async Task WriteReadabilityWasmInputToStdIn(string stdIn, string html, ReadabilityOptions? options)
    {
        using var streamWriter = new StreamWriter(stdIn, false, Encoding.UTF8);

        var wasmInput = new ReadabilityWasmInput(html, options ?? ReadabilityOptions.Empty());

        var wasmInputJson = JsonSerializer.Serialize(wasmInput, ReadabilityJsonSerializerContext.Default.ReadabilityWasmInput);

        await streamWriter.WriteAsync(wasmInputJson).ConfigureAwait(false);
        await streamWriter.FlushAsync().ConfigureAwait(false);
    }

    internal class ReadabilityWasmInput(string html, ReadabilityOptions options)
    {
        public string Html { get; } = html;
        public ReadabilityOptions Options { get; } = options;
    }
}
