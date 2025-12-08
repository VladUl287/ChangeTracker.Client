using BenchmarkDotNet.Running;
using Tracker.Benchmarks;

BenchmarkRunner.Run<ReferenceEqualVsManuallStringCompare>();
return;

BenchmarkRunner.Run<ETagComparerBenchmark>();
