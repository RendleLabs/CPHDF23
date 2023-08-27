using System.Reflection;
using Microsoft.Extensions.Logging.Abstractions;

namespace UfoData.Tests;

public class StringParserTests
{
    [Fact]
    public async Task Parses()
    {
        await using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UfoData.Tests.Data.DataHerb.csv");
        Assert.NotNull(stream);
        var target = new StringParser(NullLogger<StringParser>.Instance);
        var list = await target.Parse(stream);
        Assert.NotEmpty(list);
    }
}