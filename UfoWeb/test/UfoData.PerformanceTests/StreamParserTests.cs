using BenchmarkDotNet.Reports;

namespace UfoData.PerformanceTests;

public class StreamParserTests : IClassFixture<BenchmarkFixture>
{
    private readonly Summary _summary;

    public StreamParserTests(BenchmarkFixture benchmarkFixture)
    {
        _summary = benchmarkFixture.GetSummary();
    }
    
    [Fact]
    public void RunsInUnder90PercentOfBaseline()
    {
        var baseline = _summary.Reports.FirstOrDefault(r =>
            r.BenchmarkCase.Descriptor.DisplayInfo.StartsWith("ParserBenchmarks")
            && r.BenchmarkCase.Descriptor.Baseline);
        Assert.NotNull(baseline?.ResultStatistics);
        
        var report = _summary.Reports.FirstOrDefault(r =>
            r.BenchmarkCase.Descriptor.DisplayInfo == "ParserBenchmarks.StreamParser");
        
        Assert.NotNull(report?.ResultStatistics);

        var ratio = (report.ResultStatistics.Mean / baseline.ResultStatistics.Mean) * 100;
        
        Assert.InRange(ratio, 0d, 90d);
        
        // var mean = NanoToMilliseconds(report.ResultStatistics.Mean);
        //
        // Assert.InRange(mean, 0d, 600d);
    }

    private static double NanoToMilliseconds(double nanoseconds) =>
        nanoseconds / 1_000_000d;
}