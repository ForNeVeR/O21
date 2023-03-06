using O21.StreamUtil;

namespace O21.WinHelp;

[Flags]
public enum BTreeFlags : ushort
{
    Default = 0x0002,
    Directory = 0x0400
}

public struct BTreeHeader
{
    private const int StructureSize = 16;

    public ushort Magic;
    public BTreeFlags Flags;
    public ushort PageSize;
    public unsafe fixed byte Structure[StructureSize];
    public short FirstLeaf;
    public short PageSplits;
    public short RootPage;
    public short FirstFree;
    public short TotalPages;
    public short NLevels;
    public uint TotalHfsEntries;

    public static unsafe BTreeHeader Load(Stream data)
    {
        BTreeHeader header;
        header.Magic = data.ReadUInt16Le();
        header.Flags = (BTreeFlags)data.ReadUInt16Le();
        header.PageSize = data.ReadUInt16Le();

        var structure = new Span<byte>(header.Structure, StructureSize);
        data.ReadExactly(structure);

        header.FirstLeaf = data.ReadInt16Le();
        header.PageSplits = data.ReadInt16Le();
        header.RootPage = data.ReadInt16Le();
        header.FirstFree = data.ReadInt16Le();
        header.TotalPages = data.ReadInt16Le();
        header.NLevels = data.ReadInt16Le();
        header.TotalHfsEntries = data.ReadUInt32Le();

        return header;
    }
}
