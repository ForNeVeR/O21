using System.Buffers.Binary;

namespace O21.StreamUtil;

internal static class StreamEx
{
    public static ushort ReadUInt16Le(this Stream stream)
    {
        Span<byte> buffer = stackalloc byte[2];
        stream.ReadExactly(buffer);
        return BinaryPrimitives.ReadUInt16LittleEndian(buffer);
    }

    public static int ReadInt32Le(this Stream stream)
    {
        Span<byte> buffer = stackalloc byte[4];
        stream.ReadExactly(buffer);
        return BinaryPrimitives.ReadInt32LittleEndian(buffer);
    }

    public static byte ReadByteExact(this Stream stream)
    {
        var result = stream.ReadByte();
        if (result == -1) throw new IOException("End of stream encountered");
        return (byte)result;
    }
}
