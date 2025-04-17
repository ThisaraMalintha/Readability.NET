using Readability.NET.Tests.Helpers;
using Readability.NET.Wasm;

namespace Readability.NET.Tests;

[TestFixture]
public class ReadabilityWasmModuleTests
{
    private IReadabilityWasmModule _wasm;

    [SetUp]
    public void Setup()
    {
        _wasm = new ReadabilityWasmModule();
    }

    [TestCase("./TestPages/good-1.html")]
    public async Task Invoke_Should_Be_Successful_For_Good_Input(string htmlFile)
    {
        var fileContent = await FileHelpers.GetFileContent(htmlFile);

        var result = await _wasm.Invoke(fileContent);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Content, Is.Not.Null);
        });
    }

    [TestCase("./TestPages/bad-1.html")]
    public async Task Invoke_Should_Be_Return_Failed_Result_For_Bad_Input(string htmlFile)
    {
        var fileContent = await FileHelpers.GetFileContent(htmlFile);

        var result = await _wasm.Invoke(fileContent);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Content, Is.Null);
        });
    }
}