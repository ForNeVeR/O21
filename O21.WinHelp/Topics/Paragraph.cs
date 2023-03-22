using System.Text;
using O21.StreamUtil;

namespace O21.WinHelp.Topics;

public enum ParagraphRecordType : byte
{
    TopicHeader = 0x02,
    TextRecord = 0x20,
    TableRecord = 0x23
}

public struct Paragraph
{
    private Stream Data { get; init; }
    private long DataOffset { get; init; }

    public int BlockSize;
    public int DataLen2;
    public int PrevPara;
    public int NextPara;
    public int DataLen1;
    public ParagraphRecordType RecordType;


    public static Paragraph Load(Stream input)
    {
        return new Paragraph
        {
            Data = input,
            BlockSize = input.ReadInt32Le(),
            DataLen2 = input.ReadInt32Le(),
            PrevPara = input.ReadInt32Le(),
            NextPara = input.ReadInt32Le(),
            DataLen1 = input.ReadInt32Le(),
            RecordType = (ParagraphRecordType)input.ReadByteExact(),
            DataOffset = input.Position
        };
    }

    public byte[] ReadData1()
    {
        var realLength = DataLen1 - 21;
        Data.Position = DataOffset;
        var buffer = new byte[realLength];
        Data.ReadExactly(buffer);
        return buffer;
    }

    public byte[] ReadData2()
    {
        Data.Position = DataOffset + DataLen1 - 21;
        var buffer = new byte[DataLen2];
        Data.ReadExactly(buffer);
        return buffer;
    }

    public ParagraphItems ReadItems(Encoding encoding)
    {
        var data1 = ReadData1();
        using var data1Stream = new MemoryStream(data1);
        var header = FormatHeader.Read(data1Stream);
        var formattingData = data1[^header.FormatSize..];

        var textData = ReadData2();
        var result = new List<IParagraphItem>();

        int blockBegin = 0, blockEnd = 0;
        while (blockEnd++ < textData.Length)
        {
            if (textData[blockEnd] == 0)
            {
                YieldCurrentTextBlock();
                YieldFormatInfo();
                blockBegin = blockEnd + 1;
            }
        }

        if (blockBegin != blockEnd)
        {
            YieldCurrentTextBlock();
        }

        throw new Exception("TODO");

        // return result;

        void YieldCurrentTextBlock()
        {
            var text = encoding.GetString(textData, blockBegin, blockEnd);
            var textBlock = new ParagraphText(text);
            result.Add(textBlock);
        }

        void YieldFormatInfo()
        {
            var formatInfo = ReadFormatInfo();
            result.Add(formatInfo);
        }

        IParagraphItem ReadFormatInfo()
        {
            throw new Exception("TODO");
        }
    }
}
