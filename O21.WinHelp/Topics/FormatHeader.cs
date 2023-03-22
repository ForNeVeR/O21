using O21.StreamUtil;

namespace O21.WinHelp.Topics;

public struct FormatHeader
{
    public ushort FormatSize;
    public byte Flags;
    public ushort DataSize;

    public static FormatHeader Read(Stream input)
    {
        FormatHeader header;
        header.FormatSize = ReadDoubledValue();
        header.Flags = input.ReadByteExact();
        header.DataSize = ReadDoubledValue();
        return header;

        ushort ReadDoubledValue()
        {
            checked
            {
                var value = (ushort)input.ReadByteExact();
                if ((value & 1) != 0)
                {
                    var highByte = input.ReadByteExact();
                    value |= (ushort)(highByte << 8);
                }

                return (ushort)(value / 2);
            }
        }
    }
}
