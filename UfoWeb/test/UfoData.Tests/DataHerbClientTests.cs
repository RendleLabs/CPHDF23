using Microsoft.Extensions.Logging.Abstractions;

namespace UfoData.Tests;

public class DataHerbClientTests
{
    [Fact]
    public async Task GetsDataUsingStringParser()
    {
        var target = new DataHerbClient(new StringParser(NullLogger<StringParser>.Instance), NullLogger<DataHerbClient>.Instance);
        var actual = await target.GetSightings();
        Assert.NotEmpty(actual);
    }
}