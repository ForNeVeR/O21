using System.Runtime.InteropServices;
using O21.StreamUtil;
using Oxage.Wmf;

namespace O21.MRB;

public class MrbFile
{
    private readonly Stream _input;
    private readonly ShgFileHeader _header;

    public MrbFile(Stream input, ShgFileHeader header)
    {
        _input = input;
        _header = header;
    }

    public static MrbFile Load(Stream input)
    {
        var header = ShgFileHeader.Read(input);
        return new MrbFile(input, header);
    }

    public short ImageCount => _header.ObjectCount;

    public ShgImageHeader ReadImage(int index)
    {
        var offset = _header.ObjectOffsets[index];
        _input.Position = offset;
        return ShgImageHeader.Read(_input);
    }

    public WmfDocument ReadWmfDocument(ShgImageHeader imageHeader)
    {
        if (imageHeader.Type != ImageType.Wmf) throw new Exception("Only WMF images are supported for now");

        _input.Position = imageHeader.DataOffset;
        var metafileHeader = ShgMetafileHeader.Read(_input);

        using var result = new MemoryStream();

        // Write WMF file header:
        result.Write(new byte[] { 0xD7, 0xCD, 0xC6, 0x9A }); // magic
        result.WriteUInt16Le(0); // no idea

        result.WriteUInt16Le(0); // left
        result.WriteUInt16Le(0); // top
        result.WriteUInt16Le(metafileHeader.Width);
        result.WriteUInt16Le(metafileHeader.Height);
        result.WriteUInt16Le(2540); // default DPI for WMF
        result.WriteUInt32Le(0); // reserved

        var checksum = CalculateChecksum(result);
        result.Position = result.Length;
        result.WriteUInt16Le(checksum);

        if (imageHeader.Compression != CompressionType.Rle)
            throw new Exception($"Compression type {imageHeader.Compression} is not supported.");

        DecompressRle(metafileHeader.CompressedDataSize, result);

        result.Position = 0;
        var doc = new WmfDocument();
        doc.Load(result);
        return doc;

        ushort CalculateChecksum(Stream stream)
        {
            var position = stream.Position;
            stream.Position = 0L;

            const int length = 20;

            Span<byte> underChecksum = stackalloc byte[length];
            stream.ReadExactly(underChecksum);

            var grouped = MemoryMarshal.Cast<byte, ushort>(underChecksum);

            ushort sum = 0;
            for (var i = 0; i < length / 2; ++i)
            {
                sum ^= grouped[i];
            }

            stream.Position = position;
            return sum;
        }
    }

    public void DecompressRle(uint compressedDataSize, Stream output)
    {
        var bytesRead = 0;
        while (bytesRead < compressedDataSize)
        {
            var count = _input.ReadByteExact();
            ++bytesRead;
            if ((count & 0x80) != 0)
            {
                count -= 0x80;
                while (count-- > 0)
                {
                    var data = _input.ReadByteExact();
                    ++bytesRead;
                    output.WriteByte(data);
                }
            }
            else
            {
                count = (byte)(count & 0x7F);
                var data = _input.ReadByteExact();
                ++bytesRead;
                for (var i = 0; i < count; ++i)
                    output.WriteByte(data);
            }
        }
    }
}
