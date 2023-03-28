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

    public static BTreeIndexHeader Load(Stream data, Encoding fileNameEncoding)
    {
        BTreeIndexHeader header;
        header.Unused = data.ReadUInt16Le();
        header.NEntries = data.ReadInt16Le();
        header.PreviousPage = data.ReadInt16Le();
        header.NextPage = data.ReadInt16Le();

        header.Entries = new DirectoryIndexEntry[header.NEntries];
        for (var i = 0; i < header.NEntries; ++i)
            header.Entries[i] = DirectoryIndexEntry.Load(data, fileNameEncoding);

        return header;
    }
}

public struct DirectoryIndexEntry
{
    public string FileName;
    public int FileOffset;

    public static DirectoryIndexEntry Load(Stream data, Encoding fileNameEncoding)
    {
        var start = data.Position;
        while (data.ReadByte() != 0)
        {
        }
        var end = data.Position;

        var buffer = new byte[end - start];
        data.Position = start;
        data.ReadExactly(buffer);

        return new DirectoryIndexEntry
        {
            // -1 for terminating zero
            FileName = fileNameEncoding.GetString(buffer, 0, buffer.Length - 1),
            FileOffset = data.ReadInt32Le()
        };
    }
}
