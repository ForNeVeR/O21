namespace O21.NE;

public record struct NeResourceType(ushort TypeId, NeResource[] Resources);


/// <summary>NE resource table entry.</summary>
/// <param name="ContentOffsetInAlignments">
/// Content offset relative to beginning of the file, in terms of the alignment shift count.
/// </param>
/// <param name="ContentLength">
/// Content length, documented as size in bytes, but actually in terms of alignment shift count, too.
/// </param>
public record struct NeResource(ushort ContentOffsetInAlignments, ushort ContentLength);
