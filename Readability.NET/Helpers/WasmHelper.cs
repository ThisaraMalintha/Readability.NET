using Wasmtime;

namespace Readability.NET.Helpers;

internal class WasmHelper
{
    private const string WasmFileResourceKey = "Readability.NET.lib.dist.mozilla-readability.wasm";

    private static readonly Engine _engine = new();

    public static async Task<string> InvokeModule(string html)
    {
        var instanceId = Guid.NewGuid().ToString();

        var stdIn = $"{instanceId}_stdin.f";
        var stdOut = $"{instanceId}_stdout.f";
        var stdError = $"{instanceId}_stderr.f";

        using (var streamWriter = new StreamWriter(stdIn, false, Encoding.UTF8))
        {
            await streamWriter.WriteAsync(html).ConfigureAwait(false);
            await streamWriter.FlushAsync().ConfigureAwait(false);
        }

        try
        {
            await Task.Run(() => InvokeWasm(stdIn, stdOut, stdError))
                .ConfigureAwait(false);

            using var streamReader = new StreamReader($"{instanceId}_stdout.f");

            return await streamReader.ReadToEndAsync();
        }
        catch (Exception ex)
        {
            using var errReader = new StreamReader(stdError, Encoding.UTF8);
            var err = await errReader.ReadToEndAsync();

            throw new Exception(err, ex);
        }
        finally
        {
            File.Delete(stdIn);
            File.Delete(stdOut);
            File.Delete(stdError);
        }
    }

    private static void InvokeWasm(string stdIn, string stdOut, string stdError)
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
}
