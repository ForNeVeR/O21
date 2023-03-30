using O21.StreamUtil;

namespace O21.MRB;

public struct ShgMetafileHeader
{
    public ushort Width;
    public ushort Height;
    public uint UncompressedDataSize;
    public uint CompressedDataSize;
    public uint HotSpotDataSize;
    public uint Unknown; // documented as dwPictureOffset in splitmrb of helpdeco
    public uint HotSpotOffset;

    public static ShgMetafileHeader Read(Stream input)
    {
        ShgMetafileHeader header;
        header.Width = input.ReadUInt16Le();
        header.Height = input.ReadUInt16Le();
        header.UncompressedDataSize = input.ReadCompressedUInt32();
        header.CompressedDataSize = input.ReadCompressedUInt32();
        header.HotSpotDataSize = input.ReadCompressedUInt32();
        header.Unknown = input.ReadUInt32Le();
        header.HotSpotOffset = input.ReadUInt32Le();
        return header;
    }
}
