using System.Data;
using System.Text;
using O21.StreamUtil;

namespace O21.WinHelp;

public struct BTreeIndexHeader
{
    public ushort Unused;
    public short NEntries;
    public short PreviousPage;
    public short NextPage;
    public DirectoryIndexEntry[] Entries;

    public static BTreeIndexHeader Load(Stream data)
    {
        BTreeIndexHeader header;
        header.Unused = data.ReadUInt16Le();
        header.NEntries = data.ReadInt16Le();
        header.PreviousPage = data.ReadInt16Le();
        header.NextPage = data.ReadInt16Le();

        header.Entries = new DirectoryIndexEntry[header.NEntries];
        for (var i = 0; i < header.NEntries; ++i)
            header.Entries[i] = DirectoryIndexEntry.Load(data);

        return header;
    }
}

public struct DirectoryIndexEntry
{
    public string FileName;
    public int FileOffset;

    public static DirectoryIndexEntry Load(Stream data)
    {
        var start = data.Position;
        while (data.ReadByte() != 0)
        {
        }
        var end = data.Position;

        var buffer = new byte[end - start];
        data.Seek(start, SeekOrigin.Begin);
        data.ReadExactly(buffer);

        return new DirectoryIndexEntry
        {
            FileName = Encoding.UTF8.GetString(buffer), // TODO: Figure out the encoding
            FileOffset = data.ReadInt32Le()
        };
    }
}
