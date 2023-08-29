using System.Diagnostics;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging.Abstractions;

namespace UfoData.Benchmarks;

[MemoryDiagnoser]
public class ParserBenchmarks
{
    [Benchmark(Baseline = true)]
    public async Task<int> StringParser()
    {
        await using var stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("UfoData.Benchmarks.Data.DataHerb.csv");
        Debug.Assert(stream is not null);

        var target = new StringParser(NullLogger<StringParser>.Instance);
        var list = await target.Parse(stream);
        return list.Count;
    }

    [Benchmark]
    public async Task<int> StreamParser()
    {
        await using var stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("UfoData.Benchmarks.Data.DataHerb.csv");
        Debug.Assert(stream is not null);

        var target = new StreamParser(NullLogger<StreamParser>.Instance);
        var list = await target.Parse(stream);
        return list.Count;
    }
}