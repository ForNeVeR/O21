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
}
