using Readability.NET.Tests.Helpers;

namespace Readability.NET.Tests;

[TestFixture]
public class SmokeTests
{
    private IReadability _readability;

    [SetUp]
    public void Setup()
    {
        _readability = new Readability();
    }

    [TestCase("./TestFiles/dotnet-1.html")]
    public async Task ParseHtml_Should_Return_Successful_Readability_Result(string file)
    {
        var fileContent = await FileHelpers.GetFileContent(file);

        var result = await _readability.ParseHtml(fileContent);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Content, Is.Not.Null);
        });
    }
}