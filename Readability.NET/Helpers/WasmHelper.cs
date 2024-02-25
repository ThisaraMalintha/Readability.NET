using Wasmtime;

namespace Readability.NET.Helpers;

internal class WasmHelper
{
    private static readonly WasiConfiguration _wasiConfiguration =
        new WasiConfiguration()
            .WithStandardInput("stdin.f")
            .WithStandardOutput("stdout.f")
            .WithInheritedStandardError();

    private static readonly Engine _engine = new();

    public static async Task<string> InvokeModule(string html)
    {
        using (var streamWriter = new StreamWriter("stdin.f"))
        {
            await streamWriter.WriteAsync(html).ConfigureAwait(false);
            await streamWriter.FlushAsync().ConfigureAwait(false);
        }

        using (var store = new Store(_engine))
        using (var linker = new Linker(_engine))
        using (var module = Module.FromFile(_engine, @"./lib/dist/mozilla-readability.wasm"))
        {
            store.SetWasiConfiguration(_wasiConfiguration);
            linker.DefineWasi();

            var instance = linker.Instantiate(store, module);
            var javyStart = instance.GetAction("_start"); // Javy default startup function

            javyStart();
        }

        using var streamReader = new StreamReader("stdout.f");
        return await streamReader.ReadToEndAsync();
    }
}
