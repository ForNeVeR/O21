using O21.StreamUtil;

namespace O21.WinHelp.Fonts;

[Flags]
public enum FontAttributes : byte
{
    Normal = 0x00,
    Bold = 0x01,
    Italic = 0x02,
    Underline = 0x04,
    Strikethrough = 0x08,
    DoubleUnderline = 0x10,
    SmallCaps = 0x20
}

public struct RgbTriple
{
    public byte B;
    public byte G;
    public byte R;
}

public struct FontDescriptor
{
    public FontAttributes Attributes;
    public byte HalfPoints;
    public byte FontFamily;
    public byte FontName;
    public byte UnknownZero;
    public RgbTriple ScrollingRegionColor;
    public RgbTriple NonScrollingRegionColor;

    public static FontDescriptor Read(Stream input)
    {
        FontDescriptor descriptor;
        descriptor.Attributes = (FontAttributes)input.ReadByteExact();
        descriptor.HalfPoints = input.ReadByteExact();
        descriptor.FontFamily = input.ReadByteExact();
        descriptor.FontName = input.ReadByteExact();
        descriptor.UnknownZero = input.ReadByteExact();
        descriptor.ScrollingRegionColor.B = input.ReadByteExact();
        descriptor.ScrollingRegionColor.G = input.ReadByteExact();
        descriptor.ScrollingRegionColor.R = input.ReadByteExact();
        descriptor.NonScrollingRegionColor.B = input.ReadByteExact();
        descriptor.NonScrollingRegionColor.G = input.ReadByteExact();
        descriptor.NonScrollingRegionColor.R = input.ReadByteExact();
        return descriptor;
    }
}
