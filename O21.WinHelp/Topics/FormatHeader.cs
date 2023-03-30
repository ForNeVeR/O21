using O21.StreamUtil;

namespace O21.WinHelp.Topics;

public struct FormatHeader
{
    public ushort FormatSize;
    public byte Flags;
    public ushort DataSize;

    public static FormatHeader Read(Stream input)
    {
        FormatHeader header;
        header.FormatSize = input.ReadCompressedUInt16();
        header.Flags = input.ReadByteExact();
        header.DataSize = input.ReadCompressedUInt16();
        return header;
    }
}
