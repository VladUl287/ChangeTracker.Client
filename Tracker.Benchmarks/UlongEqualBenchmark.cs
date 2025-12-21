using BenchmarkDotNet.Attributes;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Tracker.Benchmarks;

public class UlongEqualBenchmark
{
    private static readonly ulong Ticks = (ulong)DateTimeOffset.UtcNow.Ticks;
    private readonly string TicksRaw = Ticks.ToString();

    private static readonly int ULongMaxLength = ulong.MaxValue.ToString().Length;

    [Benchmark]
    public bool EqualULongBase()
    {
        var chars = TicksRaw.AsSpan();

        if (chars.Length == 0 || chars.Length > ULongMaxLength)
            return false;

        ulong result = 0;
        foreach (var c in chars)
        {
            if (c < '0' || c > '9') return false;
            result = result * 10 + (uint)(c - '0');
        }

        return result == Ticks;
    }

    [Benchmark]
    public bool EqualULongBaseUnsafe()
    {
        var chars = TicksRaw.AsSpan();

        if (chars.Length == 0 || chars.Length > ULongMaxLength)
            return false;

        ulong result = 0;
        foreach (var c in chars)
            result = result * 10 + (uint)(c - '0');

        return result == Ticks;
    }

    [Benchmark]
    public bool EqualULongBaseUnsafeUlong()
    {
        var chars = TicksRaw.AsSpan();

        if (chars.Length == 0 || chars.Length > ULongMaxLength)
            return false;

        ulong result = 0;
        foreach (var c in chars)
            result = result * 10 + (ulong)(c - '0');

        return result == Ticks;
    }

    [Benchmark]
    public bool EqualULongUnsafe()
    {
        var chars = TicksRaw.AsSpan();

        if (chars.Length == 0 || chars.Length > ULongMaxLength)
            return false;

        ulong result = 0;
        ref char ptr = ref MemoryMarshal.GetReference(chars);

        for (int i = 0; i < chars.Length; i++)
        {
            var digit = (uint)(Unsafe.Add(ref ptr, i) - '0');
            result = result * 10 + digit;
        }

        return result == Ticks;
    }
}
