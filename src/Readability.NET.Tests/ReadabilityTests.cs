namespace Readability.NET.Tests;

[TestFixture]
public class ReadabilityTests
{
    private Readability _readability;

    [SetUp]
    public void Setup()
    {
        _readability = new Readability();
    }

    [TearDown]
    public void TearDown()
    {
        _readability.Dispose();
    }

    [TestCase("./TestPages/good-1.html")]
    public async Task ParseHtml_Should_Be_Successful_For_Good_Input(string htmlFile)
    {
        var fileContent = await FileHelpers.GetFileContent(htmlFile);

        var result = await _readability.ParseHtml(fileContent);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Content, Is.Not.Null);
        });
    }

    [TestCase("./TestPages/bad-1.html")]
    public async Task ParseHtml_Should_Fail_For_Bad_Input(string htmlFile)
    {
        var fileContent = await FileHelpers.GetFileContent(htmlFile);

        var result = await _readability.ParseHtml(fileContent);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Content, Is.Null);
        });
    }

    [Test]
    public async Task ParseUrl_Should_Be_Successful_For_Wikipedia_Mozilla()
    {
        const string url = "https://github.com/mozilla/readability";

        var result = await _readability.ParseUrl(url);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Content, Is.Not.Null.And.Not.Empty);
        });
    }
}
