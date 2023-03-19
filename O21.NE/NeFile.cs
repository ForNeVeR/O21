using O21.StreamUtil;

namespace O21.NE;

/// <remarks>https://jeffpar.github.io/kbarchive/kb/065/Q65122/</remarks>
public class NeFile
{
    private readonly Stream _input;
    private readonly ushort _segmentedHeaderOffset;
    private readonly ushort _resourceTableOffset;
    private readonly ushort _resourceAlignmentShiftCount;

    public NeFile(
        Stream input,
        ushort segmentedHeaderOffset,
        ushort resourceTableOffset,
        ushort resourceAlignmentShiftCount)
    {
        _input = input;
        _segmentedHeaderOffset = segmentedHeaderOffset;
        _resourceTableOffset = resourceTableOffset;
        _resourceAlignmentShiftCount = resourceAlignmentShiftCount;
    }

    public static NeFile ReadFrom(Stream input)
    {
        input.Position = 0x3CL;
        var segmentedHeaderOffset = input.ReadUInt16Le();
        input.Position = segmentedHeaderOffset;

        Span<byte> signature = stackalloc byte[2];
        input.ReadExactly(signature);
        if (!signature.SequenceEqual("NE"u8))
            throw new Exception(
                $"""Invalid NE file signature: "{(char)signature[0]}{(char)signature[1]}" instead of "NE".""");

        // Read resource table offset at header + 0x24:
        input.Position = segmentedHeaderOffset + 0x24;
        var resourceTableOffset = input.ReadUInt16Le();

        // The resource entry number is supposed to be placed at header + 0x34, though it's always zero in
        // the files I examined. Let's ignore it: the resource table has its own end marker (typeId == 0).
        //
        // Notably, eXeScope calls the same field "Number of Reserved Segment".

        // Read resource alignment shift count at resource table offset:
        input.Position = segmentedHeaderOffset + resourceTableOffset;
        var alignmentShiftCount = input.ReadUInt16Le();

        return new NeFile(input, segmentedHeaderOffset, resourceTableOffset, alignmentShiftCount);
    }

    public IEnumerable<NeResourceType> ReadResourceTable()
    {
        _input.Position = _segmentedHeaderOffset + _resourceTableOffset + 2;
        while (true)
        {
            var typeId = _input.ReadUInt16Le();
            if ((typeId & (1 << 16)) != 0)
                throw new Exception($"Non-integer resource type id is not supported: {typeId}.");

            if (typeId == 0) yield break;

            var resourceCount = _input.ReadUInt16Le();
            _input.Position += 4; // DD Reserved

            var resources = new NeResource[resourceCount];
            for (var r = 0; r < resourceCount; ++r)
            {
                resources[r] = ReadResource();
            }

            yield return new NeResourceType(typeId, resources);
        }
    }

    public byte[] ReadResourceContent(NeResource resource)
    {
        var alignment = 1 << _resourceAlignmentShiftCount;
        var offset = resource.ContentOffsetInAlignments * alignment;
        var length = resource.ContentLength * alignment;

        _input.Position = offset;
        var buffer = new byte[length];
        _input.ReadExactly(buffer);
        return buffer;
    }

    private NeResource ReadResource()
    {
        var offset = _input.ReadUInt16Le();
        var length = _input.ReadUInt16Le();

        // DW Flag word
        // DW Resource ID
        // DD Reserved
        _input.Position += 8;

        return new NeResource(offset, length);
    }
}
