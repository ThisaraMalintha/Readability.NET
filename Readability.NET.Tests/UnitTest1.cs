namespace Readability.NET.Tests;

public class Tests
{
    private IReadability _readability;

    [SetUp]
    public void Setup()
    {
        _readability = new Readability();
    }

    [Test]
    public async Task Test1()
    {
        var url = "https://www.hanselman.com/blog/the-code-worked-differently-when-the-moon-was-full?utm_source=pocket_saves";

        var result = await _readability.Parse(url);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Content, Is.Not.Null);
        });
    }
}