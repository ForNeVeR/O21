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
    public int FileSizeWithHeader;
    public int FileSize;
    public HfsFileType FileType;

    public static HfsEntry Load(Stream input) => new()
    {
        FileSizeWithHeader = input.ReadInt32Le(),
        FileSize = input.ReadInt32Le(),
        FileType = (HfsFileType)input.ReadByteExact()
    };
}
