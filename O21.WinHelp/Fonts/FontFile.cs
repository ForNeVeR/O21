namespace O21.WinHelp.Fonts;

public class FontFile
{
    private Stream _data;
    private FontHeader _header;
    public FontFile(Stream data, FontHeader header)
    {
        _data = data;
        _header = header;
    }

    public static FontFile Load(Stream stream)
    {
        var header = FontHeader.Read(stream);
        return new FontFile(stream, header);
    }

    public unsafe FontDescriptor[] ReadDescriptors()
    {
        _data.Position = _header.DescriptorsOffset;
        var result = new FontDescriptor[_header.NumDescriptors];
        for (var i = 0; i < _header.NumDescriptors; ++i)
        {
            result[i] = FontDescriptor.Read(_data);
        }
        return result;
    }
}
