using O21.StreamUtil;

namespace O21.WinHelp.Topics;

[Flags]
public enum ParagraphSetup
{
    SpaceBefore = 0x00020000,
    SpaceAfter = 0x00040000,
    LineSpacingBefore = 0x00080000,
    LeftMarginIndent = 0x00100000,
    RightMarginIndent = 0x00200000,
    FirstLineIndent = 0x00400000,
    ParagraphBorder = 0x01000000,
    TabSettingInformation = 0x02000000,
    RightJustify = 0x04000000,
    CenterJustify = 0x08000000,
    NoWrap = 0x10000000
}

[Flags]
public enum ParagraphBorder
{
    DottedBorder = 0x80,
    DoubleBorder = 0x40,
    ThickBorder = 0x20,
    RightBorder = 0x10,
    BottomBorder = 0x08,
    LeftBorder = 0x04,
    TopBorder = 0x02,
    BoxedBorder = 0x01
}

public struct ParagraphSettings
{
    public ParagraphSetup Setup;
    public ParagraphBorder? Border;

    public static ParagraphSettings Load(Stream data)
    {
        ParagraphSettings settings;
        settings.Setup = (ParagraphSetup)data.ReadInt32Le();
        if ((settings.Setup & ParagraphSetup.ParagraphBorder) != 0)
        {
            var header = data.ReadByteExact();
            if (header != 0x01) throw new Exception($"Paragraph border header: {header:x}, expected: 0x01.");
            settings.Border = (ParagraphBorder)data.ReadByteExact();
            var footer = data.ReadByteExact();
            if (footer != 0x51) throw new Exception($"Paragraph border footer: {footer:x}, expected: 0x51.");
        }
        else
        {
            settings.Border = null;
        }

        return settings;
    }
}

public interface IParagraphItem {}

public record struct ParagraphText(string Text) : IParagraphItem;

public record struct FontChange(ushort FontDescriptor) : IParagraphItem;
public record struct Newline : IParagraphItem;
public record struct NewParagraph : IParagraphItem;
public record struct Tab : IParagraphItem;
public record struct Bitmap : IParagraphItem; // TODO: Figure out how it works

public record ParagraphItems(
    ParagraphSettings Settings,
    IReadOnlyList<IParagraphItem> Items
);
