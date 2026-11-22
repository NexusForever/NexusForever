using NexusForever.IO.Format.WildStar.Shared.Array;

namespace NexusForever.IO.Format.Wildstar.DGN.Dressing
{
    public class DNGDressingFile : IBinaryReadable
    {
        // client hard codes this length
        public static uint DataOffset => 48u;

        public ulong Index { get; set; }
        public PointerReadableArray<Model> Models { get; } = new();
        public PointerReadableArray<Unknown18> Unknown18 { get; } = new();

        public void Read(BinaryReader reader)
        {
            Index = reader.ReadUInt64();
            Models.Read(reader, DataOffset);
            Unknown18.Read(reader, DataOffset);
        }
    }
}
