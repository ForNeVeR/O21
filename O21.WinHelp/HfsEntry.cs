using O21.StreamUtil;

namespace O21.WinHelp;

internal enum HfsFileType
{
    Normal = 0,
    /// <summary>Entry of the HFS itself.</summary>
    Hfs = 4
}

internal struct HfsEntry
{
    public int ReservedSpace;
    public int UsedSpace;
    public HfsFileType FileType;

    public static HfsEntry Load(Stream input) => new()
    {
        ReservedSpace = input.ReadInt32Le(),
        UsedSpace = input.ReadInt32Le(),
        FileType = (HfsFileType)input.ReadByteExact()
    };
}
