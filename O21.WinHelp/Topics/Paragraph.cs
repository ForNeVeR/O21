using System.Globalization;
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
        using var formattingDataStream = new MemoryStream(formattingData);
        var settings = ParagraphSettings.Load(formattingDataStream);

        var formattingDataBoundary = formattingDataStream.ReadByteExact();
        if (formattingDataBoundary != 0)
            throw new Exception(
                $"Invalid formatting data boundary: found {formattingDataBoundary.ToString("x", CultureInfo.InvariantCulture)}, expected 0");

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

        var lastFormattingCommand = formattingDataStream.ReadByteExact();
        if (lastFormattingCommand != 0xFF)
            throw new Exception(
                $"Malformed formatting data: last item is {lastFormattingCommand.ToString("x", CultureInfo.InvariantCulture)}, not FF.");

        return new ParagraphItems(
            settings,
            result
        );

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
            var command = formattingDataStream.ReadByteExact();
            return command switch
            {
                0x80 => new FontChange(formattingDataStream.ReadUInt16Le()),
                0x81 => new NewLine(),
                0x82 => new NewParagraph(),
                0x83 => new Tab(),
                0x86 => throw new Exception("TODO: Bitmap current"),
                // TODO: Other known item types
                _ => throw new Exception(
                    $"Unknown formatting command code: {command.ToString("x", CultureInfo.InvariantCulture)}.")
            };
        }
    }
}
