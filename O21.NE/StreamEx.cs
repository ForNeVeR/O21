using System.Buffers.Binary;

namespace O21.NE;

internal static class StreamEx
{
    public static ushort ReadUInt16Le(this Stream stream)
    {
        Span<byte> buffer = stackalloc byte[2];
        stream.ReadExactly(buffer);
        return BinaryPrimitives.ReadUInt16LittleEndian(buffer);
    }
}
