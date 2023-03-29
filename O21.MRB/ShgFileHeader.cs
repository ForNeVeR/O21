using O21.StreamUtil;

namespace O21.MRB;

public struct ShgFileHeader
{
    public short ObjectCount;
    public uint[] ObjectOffsets;

    public static ShgFileHeader Read(Stream input)
    {
        Span<byte> magic = stackalloc byte[2];
        input.ReadExactly(magic);
        if (!magic.SequenceEqual("lp"u8)) throw new Exception("Unexpected magic bytes");

        ShgFileHeader header;
        header.ObjectCount = input.ReadInt16Le();
        header.ObjectOffsets = new uint[header.ObjectCount];
        for (var i = 0; i < header.ObjectCount; i++)
        {
            header.ObjectOffsets[i] = input.ReadUInt32Le();
        }
        return header;
    }
}
