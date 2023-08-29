using BenchmarkDotNet.Running;
using UfoData.Benchmarks;

var summary = BenchmarkRunner.Run<ParserBenchmarks>();
