using NexusForever.IO.Format.WildStar.Shared.Array;

namespace NexusForever.IO.Format.WildStar.I3
{
    public class Pass : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 88u;

        public uint Unknown0 { get; set; }
        public PointerStringArray Unknown8 { get; set; } = new();
        public PointerReadableArray<PassUnknown18> Unknown18 { get; set; } = new();
        public PointerUShortArray Unknown28 { get; set; } = new();
        public PointerReadableArray<PassUnknown38> Unknown38 { get; set; } = new();
        public PointerUShortArray Unknown48 { get; set; } = new();

        public void Read(BinaryReader reader, long dataOffset)
        {
            Unknown0 = reader.ReadUInt32();
            reader.BaseStream.Position += 4;

            Unknown8.Read(reader, dataOffset);
            Unknown18.Read(reader, dataOffset);
            Unknown28.Read(reader, dataOffset);
            Unknown38.Read(reader, dataOffset);
            Unknown48.Read(reader, dataOffset);
        }
    }
}
