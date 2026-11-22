namespace NexusForever.IO.Format.WildStar.M3.Static
{
    [Flags]
    public enum VertexBlockFlags
    {
        None             = 0x0000,
        HasPosition      = 0x0001,
        HasTangent       = 0x0002,
        HasNormal        = 0x0004,
        HasBiTangent     = 0x0008,
        HasBoneIndices   = 0x0010,
        HasBoneWeights   = 0x0020,
        HasVertexColour0 = 0x0040,
        HasVertexColour1 = 0x0080,
        HasUV0           = 0x0100,
        HasUV1           = 0x0200,
        HasUnknown       = 0x0400
    }
}
