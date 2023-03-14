using O21.StreamUtil;

namespace O21.WinHelp.Topics;

public enum ParagraphRecordType : byte
{
    TopicHeader = 0x02,
    TextRecord = 0x20,
    TableRecord = 0x23
}

public struct ParagraphHeader
{
    public int BlockSize;
    public int DataLen2;
    public int PrevPara;
    public int NextPara;
    public int DataLen1;
    public ParagraphRecordType RecordType;

    public static ParagraphHeader Load(Stream input)
    {
        ParagraphHeader header;
        header.BlockSize = input.ReadInt32Le();
        header.DataLen2 = input.ReadInt32Le();
        header.PrevPara = input.ReadInt32Le();
        header.NextPara = input.ReadInt32Le();
        header.DataLen1 = input.ReadInt32Le();
        header.RecordType = (ParagraphRecordType)input.ReadByteExact();
        return header;
    }
}
