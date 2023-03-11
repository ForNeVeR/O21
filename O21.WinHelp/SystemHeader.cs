using System.Globalization;
using O21.StreamUtil;

namespace O21.WinHelp;

public struct SystemHeader
{
    public byte Magic;
    public byte Version;
    public byte Revision;
    public byte Always0;
    public ushort Always1;
    public uint GenDate;
    public ushort Flags;

    public static SystemHeader Load(Stream input)
    {
        SystemHeader header;
        header.Magic = input.ReadByteExact();
        header.Version = input.ReadByteExact();
        header.Revision = input.ReadByteExact();
        header.Always0 = input.ReadByteExact();
        header.Always1 = input.ReadByteExact();
        header.GenDate = input.ReadUInt32Le();
        header.Flags = input.ReadUInt16Le();

        if (header.Magic != 0x6C)
            throw new Exception(
                $"Magic: expected to be 0x6C, actual {header.Magic.ToString("x", CultureInfo.InvariantCulture)}");

        if (header.Version != 3)
            throw new Exception(
                $"Version: expected to be 3, actual {header.Version.ToString(CultureInfo.InvariantCulture)}");

        if (header.Always0 != 0)
            throw new Exception(
                $"Always0: expected to be 0, actual {header.Always0.ToString(CultureInfo.InvariantCulture)}");

        if (header.Always1 != 1)
            throw new Exception(
                $"Always1: expected to be 1, actual {header.Always0.ToString(CultureInfo.InvariantCulture)}");

        return header;
    }
}
