using O21.StreamUtil;

namespace O21.WinHelp.Fonts;

public struct FontHeader
{
    public ushort NumFonts;
    public ushort NumDescriptors;
    public ushort DefDescriptor;
    public ushort DescriptorsOffset;

    public static FontHeader Read(Stream input)
    {
        FontHeader header;
        header.NumFonts = input.ReadUInt16Le();
        header.NumDescriptors = input.ReadUInt16Le();
        header.DefDescriptor = input.ReadUInt16Le();
        header.DescriptorsOffset = input.ReadUInt16Le();
        return header;
    }
}
