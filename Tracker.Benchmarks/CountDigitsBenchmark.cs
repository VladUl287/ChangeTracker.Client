using BenchmarkDotNet.Attributes;
using System.Runtime.CompilerServices;

namespace Tracker.Benchmarks;

public class CountDigitsBenchmark
{
    public ulong N { get; init; } = 639019381513319689UL; //18 digits

    [Benchmark]
    public int CountDigitsLog10()
    {
        if (N == 0) return 1;
        return (int)Math.Log10(N) + 1;
    }

    [Benchmark]
    public int CountDigitsIfElse()
    {
        if (N < 10UL) return 1;
        if (N < 100UL) return 2;
        if (N < 1000UL) return 3;
        if (N < 10000UL) return 4;
        if (N < 100000UL) return 5;
        if (N < 1000000UL) return 6;
        if (N < 10000000UL) return 7;
        if (N < 100000000UL) return 8;
        if (N < 1000000000UL) return 9;
        if (N < 10000000000UL) return 10;
        if (N < 100000000000UL) return 11;
        if (N < 1000000000000UL) return 12;
        if (N < 10000000000000UL) return 13;
        if (N < 100000000000000UL) return 14;
        if (N < 1000000000000000UL) return 15;
        if (N < 10000000000000000UL) return 16;
        if (N < 100000000000000000UL) return 17;
        if (N < 1000000000000000000UL) return 18;
        if (N < 10000000000000000000UL) return 19;
        return 20;
    }

    [Benchmark]
    public int CountDigitsSwitch()
    {
        return N switch
        {
            < 10UL => 1,
            < 100UL => 2,
            < 1000UL => 3,
            < 10000UL => 4,
            < 100000UL => 5,
            < 1000000UL => 6,
            < 10000000UL => 7,
            < 100000000UL => 8,
            < 1000000000UL => 9,
            < 10000000000UL => 10,
            < 100000000000UL => 11,
            < 1000000000000UL => 12,
            < 10000000000000UL => 13,
            < 100000000000000UL => 14,
            < 1000000000000000UL => 15,
            < 10000000000000000UL => 16,
            < 100000000000000000UL => 17,
            < 1000000000000000000UL => 18,
            < 10000000000000000000UL => 19,
            _ => 20
        };
    }

    [Benchmark]
    public int CountDigitsSwitchRverse()
    {
        return N switch
        {
            >= 10000000000000000000UL => 20,
            >= 1000000000000000000UL => 19,
            >= 100000000000000000UL => 18,
            >= 10000000000000000UL => 17,
            >= 1000000000000000UL => 16,
            >= 100000000000000UL => 15,
            >= 10000000000000UL => 14,
            >= 1000000000000UL => 13,
            >= 100000000000UL => 12,
            >= 10000000000UL => 11,
            >= 1000000000UL => 10,
            >= 100000000UL => 9,
            >= 10000000UL => 8,
            >= 1000000UL => 7,
            >= 100000UL => 6,
            >= 10000UL => 5,
            >= 1000UL => 4,
            >= 100UL => 3,
            >= 10UL => 2,
            _ => 1
        };
    }

    [Benchmark]
    public int GetDigitGroupExact() => N < 10000000000UL ? 10 : 20;
}
