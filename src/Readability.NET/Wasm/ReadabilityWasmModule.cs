using Wasmtime;

namespace Readability.NET.Wasm;

public class ReadabilityWasmModule : IReadabilityWasmModule
{
    private const string WasmFileResourceKey = "Readability.NET.Wasm.lib.dist.mozilla-readability.wasm";

    private static readonly Engine _engine = new();

    // Separator for the StdOut file debug log and the actual readability result json.
    private const string DebugLogBoundary = "###DEBUG_END###";

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

            return await ReadReadabilityResultFromStdOut(stdOut, options)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            var wasmError = string.Empty;

            if (File.Exists(stdError))
            {
                using var errReader = new StreamReader(stdError, Encoding.UTF8);
                wasmError = string.Join(Environment.NewLine, File.ReadLines(stdError).Take(10_000));
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

    private static async Task<ReadabilityResult> ReadReadabilityResultFromStdOut(string stdOut, ReadabilityOptions? options)
    {
        using var streamReader = new StreamReader(stdOut);

        var stdOutText = await streamReader.ReadToEndAsync();

        if (options?.Debug == true)
        {
            return GetReadabilityResultWithDebugLog(stdOutText);
        }

        var result = JsonSerializer.Deserialize(stdOutText, ReadabilityJsonSerializerContext.Default.ReadabilityResult);

        if(result is null)
        {
            return ReadabilityResult.Fail();
        }

        result.IsSuccess = true;
        return result;
    }

    private static ReadabilityResult GetReadabilityResultWithDebugLog(string stdOutText)
    {
        var debugEndIndex = stdOutText.IndexOf(DebugLogBoundary);

        if (debugEndIndex < 0)
        {
            throw new Exception("Readability result reading failed");
        }

        var readabilityResultStartIndex = debugEndIndex + DebugLogBoundary.Length;

        var debugLog = stdOutText[..debugEndIndex];

        var readabilityResult = JsonSerializer.Deserialize(stdOutText[readabilityResultStartIndex..],
            ReadabilityJsonSerializerContext.Default.ReadabilityResult);

        if (readabilityResult is null)
        {
            return ReadabilityResult.Fail(debugLog);
        }

        readabilityResult.DebugLog = debugLog;
        readabilityResult.IsSuccess = true;

        return readabilityResult;
    }

    internal class ReadabilityWasmInput(string html, ReadabilityOptions options)
    {
        public string Html { get; } = html;
        public ReadabilityOptions Options { get; } = options;
    }
}
