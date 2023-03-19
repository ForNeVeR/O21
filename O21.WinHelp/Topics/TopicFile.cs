namespace O21.WinHelp.Topics;

public struct TopicFile
{
    private readonly Stream _data;
    private readonly TopicBlockHeader _header;
    
    public TopicFile(Stream data, TopicBlockHeader header)
    {
        _data = data;
        _header = header;
    }

    public static TopicFile Load(Stream input)
    {
        var header = TopicBlockHeader.Load(input);
        
        return new(input, header);
    }

    public List<Paragraph> ReadParagraphs()
    {
        var paragraphs = new List<Paragraph>();
        var ptr = _header.TopicData;
        while (ptr != -1)
        {
            _data.Position = ptr;
            var paragraph = Paragraph.Load(_data);
            paragraphs.Add(paragraph);
            ptr = paragraph.NextPara;
        }

        return paragraphs;
    }
}
