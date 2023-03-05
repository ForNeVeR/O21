using System.Globalization;
using O21.StreamUtil;

namespace O21.WinHelp;

/// <remarks>http://www.oocities.org/mwinterhoff/helpfile.htm</remarks>
public class WinHelpFile
{
    private const int Magic = 0x35F3F;

    public static WinHelpFile Load(Stream input)
    {
        var magic = input.ReadInt32Le();
        if (magic != Magic)
        {
            throw new Exception(
                $"Expected magic sequence {Magic.ToString("x", CultureInfo.InvariantCulture)}, " +
                $"got {magic.ToString("x", CultureInfo.InvariantCulture)}.");
        }

        var directoryStart = input.ReadInt32Le();
        var firstFreeBlock = input.ReadInt32Le();
        var entireFileSize = input.ReadInt32Le();

        return new WinHelpFile();
    }
}
