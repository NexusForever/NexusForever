using NexusForever.IO.Format.WildStar.Shared.Array;

namespace NexusForever.IO.Format.WildStar.I3
{
    public class PassUnknown38 : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 32u;

        public ushort Unknown0 { get; set; }
        public ushort Unknown2 { get; set; }
        public uint Unknown4 { get; set; }
        public float Unknown8 { get; set; }
        public PointerStringArray Unknown { get; set; } = new();

        public void Read(BinaryReader reader, long dataOffset)
        {
            Unknown0 = reader.ReadUInt16();
            Unknown2 = reader.ReadUInt16();
            Unknown4 = reader.ReadUInt32();
            Unknown8 = reader.ReadSingle();
            reader.BaseStream.Position += 4;

            Unknown.Read(reader, dataOffset);
        }
    }
}
