using NexusForever.IO.Format.WildStar;
using NexusForever.IO.Format.WildStar.Shared.Array;

namespace NexusForever.IO.Format.Wildstar.DGN
{
    public class DressingInfo : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 40u;

        public ulong Index { get; set; }
        public PointerStringArray Name { get; } = new();
        public PointerStringArray File { get; } = new();

        public void Read(BinaryReader reader, long dataOffset)
        {
            Index = reader.ReadUInt64();
            Name.Read(reader, dataOffset);
            File.Read(reader, dataOffset);
        }
    }
}
