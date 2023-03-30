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

    public static short ReadInt16Le(this Stream stream)
    {
        Span<byte> buffer = stackalloc byte[2];
        stream.ReadExactly(buffer);
        return BinaryPrimitives.ReadInt16LittleEndian(buffer);
    }

    public static uint ReadUInt32Le(this Stream stream)
    {
        Span<byte> buffer = stackalloc byte[4];
        stream.ReadExactly(buffer);
        return BinaryPrimitives.ReadUInt32LittleEndian(buffer);
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

    public static ushort ReadCompressedUInt16(this Stream stream)
    {
        checked
        {
            var value = (ushort)stream.ReadByteExact();
            if ((value & 1) != 0)
            {
                var highByte = stream.ReadByteExact();
                value |= (ushort)(highByte << 8);
            }

            return (ushort)(value / 2);
        }
    }

    public static uint ReadCompressedUInt32(this Stream stream)
    {
        checked
        {
            var value = (uint)stream.ReadUInt16Le();
            if ((value & 1) != 0)
            {
                var highUInt16Le = stream.ReadUInt16Le();
                value |= (uint)(highUInt16Le << 16);
            }

            return (ushort)(value / 2);
        }
    }

    public static void WriteUInt16Le(this Stream stream, ushort value)
    {
        Span<byte> buffer = stackalloc byte[2];
        BinaryPrimitives.WriteUInt16LittleEndian(buffer, value);
        stream.Write(buffer);
    }

    public static void WriteUInt32Le(this Stream stream, uint value)
    {
        Span<byte> buffer = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(buffer, value);
        stream.Write(buffer);
    }
}
