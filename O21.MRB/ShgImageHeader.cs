using O21.StreamUtil;

namespace O21.MRB;

public enum ImageType : byte
{
    Bmp = 0x06,
    Wmf = 0x08
}

public enum CompressionType : byte
{
    None = 0,
    Rle = 1,
    Lz77 = 2
}

public struct ShgImageHeader
{
    public ImageType Type;
    public CompressionType Compression;
    public ushort Dpi;

    public static ShgImageHeader Read(Stream input)
    {
        ShgImageHeader header;
        header.Type = (ImageType)input.ReadByteExact();
        header.Compression = (CompressionType)input.ReadByteExact();
        header.Dpi = ReadCompressedValue();
        return header;

        ushort ReadCompressedValue()
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
