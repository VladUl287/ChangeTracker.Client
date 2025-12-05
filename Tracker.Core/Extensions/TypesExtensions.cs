using System.Buffers;
using System.Buffers.Binary;
using System.IO.Hashing;
using System.Text;

namespace Tracker.Core.Extensions;

public static class TypesExtensions
{
    public static string GetTypeHashId(this Type type)
    {
        var typeName = type.FullName ?? throw new NullReferenceException();

        var maximumBytes = Encoding.UTF8.GetMaxByteCount(typeName.Length);

        const int MaxBytesThreshold = 256;
        if (maximumBytes > MaxBytesThreshold)
        {
            byte[] data = ArrayPool<byte>.Shared.Rent(maximumBytes);

            var count = Encoding.UTF8.GetBytes(typeName, data);
            var bytes = data.AsSpan()[..count];

            var hash = XxHash64.HashToUInt64(bytes);

            ArrayPool<byte>.Shared.Return(data);

            Span<byte> encodedBytes = stackalloc byte[8];
            BinaryPrimitives.WriteUInt64LittleEndian(encodedBytes, hash);

            return Convert.ToBase64String(encodedBytes);
        }
        else
        {
            Span<byte> data = stackalloc byte[maximumBytes];

            var count = Encoding.UTF8.GetBytes(typeName, data);
            data = data[..count];

            var hash = XxHash64.HashToUInt64(data);

            Span<byte> encodedBytes = stackalloc byte[8];
            BinaryPrimitives.WriteUInt64LittleEndian(encodedBytes, hash);

            return Convert.ToBase64String(encodedBytes);
        }
    }
}
