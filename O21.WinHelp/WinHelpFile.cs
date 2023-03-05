using System.Globalization;
using O21.StreamUtil;

namespace O21.WinHelp;

/// <remarks>
/// <para>[1]: http://www.oocities.org/mwinterhoff/helpfile.htm</para>
/// <para>[2]: P. Davis and M. Wallace — Windows Undocumented File Formats.</para>
/// </remarks>
public struct WinHelpFile
{
    private const int Magic = 0x35F3F;

    private readonly Stream _data;

    private readonly int _hfsOffset;

    /// <remarks>This is marked as "Reserved, -1" in [2], but has more documentation in [1].</remarks>
    private readonly int _firstFreeBlock;

    private readonly int _entireFileSize;

    public WinHelpFile(Stream data, int hfsOffset, int firstFreeBlock, int entireFileSize)
    {
        _data = data;
        _hfsOffset = hfsOffset;
        _firstFreeBlock = firstFreeBlock;
        _entireFileSize = entireFileSize;
    }

    public static WinHelpFile Load(Stream input)
    {
        var magic = input.ReadInt32Le();
        if (magic != Magic)
        {
            throw new Exception(
                $"Expected magic sequence {Magic.ToString("x", CultureInfo.InvariantCulture)}, " +
                $"got {magic.ToString("x", CultureInfo.InvariantCulture)}.");
        }

        var hfsOffset = input.ReadInt32Le();
        var firstFreeBlock = input.ReadInt32Le();
        var entireFileSize = input.ReadInt32Le();

        return new WinHelpFile(input, hfsOffset, firstFreeBlock, entireFileSize);
    }

    public IEnumerable<string> HfsFileNames()
    {
        _data.Seek(_hfsOffset, SeekOrigin.Begin);
        var hfs = HfsEntry.Load(_data);
        if (hfs.FileType != HfsFileType.Hfs) throw new Exception($"Unexpected root file entry: {hfs.FileType}.");

        throw new Exception("TODO: Finish this method.");
    }
}
