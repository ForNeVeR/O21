using O21.StreamUtil;

namespace O21.WinHelp.Topics;

public struct TopicBlockHeader
{
    public int LastParagraph;
    public int TopicData;
    public int LastTopicHeader;

    public static TopicBlockHeader Load(Stream input)
    {
        TopicBlockHeader header;
        header.LastParagraph = input.ReadInt32Le();
        header.TopicData = input.ReadInt32Le();
        header.LastTopicHeader = input.ReadInt32Le();
        return header;
    }
}
