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

    public IEnumerable<ParagraphHeader> ReadParagraphs()
    {
        var ptr = _header.TopicData;
        while (ptr != -1)
        {
            _data.Seek(ptr, SeekOrigin.Begin);
            var paragraph = ParagraphHeader.Load(_data);
            yield return paragraph;
            ptr = paragraph.NextPara;
        }
    }
}
