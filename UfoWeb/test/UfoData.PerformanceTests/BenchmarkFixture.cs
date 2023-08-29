using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Filters;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using UfoData.Benchmarks;

namespace UfoData.PerformanceTests;

public class BenchmarkFixture
{
    private readonly object _mutex = new();
    private Summary? _summary;
    
    public Summary GetSummary()
    {
        lock (_mutex)
        {
            if (_summary is null)
            {
                var summary = Run();
                return _summary = summary;
            }

            return _summary;
        }
    }

    private Summary Run()
    {
#if (DEBUG)
        IConfig config = new DebugInProcessConfig();
#else
        IConfig config = new ManualConfig();
#endif
        
        config = config
            .WithSummaryStyle(SummaryStyle.Default.WithMaxParameterColumnWidth(100));
        
        return BenchmarkRunner.Run<ParserBenchmarks>(config);
    }
}