namespace O21.WinHelp.Topics;

public struct Paragraph
{
    public int BlockSize;
    public int DataLen2;
    public int PrevPara;
    public int NextPara;
    public int DataLen1;
    public byte RecordType;
    // TODO: LinkData1, LinkData2 follow 
}
