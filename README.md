# Readability.NET
WebAssembly based .NET wrapper for the Mozilla [readability](https://github.com/mozilla/readability) javascript library. Readability is the library used by the Mozilla Firefox reader view to extract readable content from html.

## How it works
1. **Javascript to WebAssembly** - Compiles the readability library to WebAssembly using the [Javy](https://github.com/bytecodealliance/javy) javascript WebAssembly toolchain.
2. **WebAssembly Execution** - [wasmtime-dotnet](https://github.com/bytecodealliance/wasmtime-dotnet) to execute the readability WebAssembly module in an embedded wasm runtime.

## Usage
```csharp
using Readability.NET;

using var readability = new Readability();

// Passing the url
var urlResult = await readability.ParseUrl("<URL>");

// Or by passing in the html content directly
var htmlResult = await readability.ParseHtml("<HTML>");
```
