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

    [TearDown]
    public void TearDown()
    {
        _wasm.Dispose();
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
    public async Task Invoke_Should_Fail_For_Bad_Input(string htmlFile)
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

    [Test]
    public void Invoke_Should_Output_Wasm_Error_Details_In_Exceptions()
    {
        const string MissingHtmlErrorMessage = "Html input not found";
        const string? BadHtmlInput = null;

        var readabilityException = Assert.ThrowsAsync<ReadabilityException>(() => _wasm.Invoke(BadHtmlInput!));

        Assert.That(readabilityException.Message, Contains.Substring(MissingHtmlErrorMessage));
    }
}