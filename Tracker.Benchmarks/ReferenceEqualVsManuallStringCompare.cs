using BenchmarkDotNet.Attributes;

namespace Tracker.Benchmarks;

[MemoryDiagnoser]
public class ReferenceEqualVsManuallStringCompare
{
    private static string FirstValue = "firstvalue";
    private static string SecondValue = FirstValue;

    [Benchmark]
    public bool Equal_Reference()
    {
        return ReferenceEquals(FirstValue, SecondValue);
    }

    [Benchmark]
    public bool Equal_ByValue()
    {
        return FirstValue == SecondValue;
    }
}
