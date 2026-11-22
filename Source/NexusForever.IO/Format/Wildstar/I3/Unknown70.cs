using NexusForever.IO.Format.WildStar.Shared.Array;

namespace NexusForever.IO.Format.WildStar.I3
{
    public class Unknown70 : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 40u;

        public PointerStringArray Unknown8 { get; set; } = new();
        public PointerUShortArray Unknown18 { get; set; } = new();

        public void Read(BinaryReader reader, long dataOffset)
        {
            reader.BaseStream.Position += 8;
            Unknown8.Read(reader, dataOffset);
            Unknown18.Read(reader, dataOffset);
        }
    }
}
