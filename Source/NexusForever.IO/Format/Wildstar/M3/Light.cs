using NexusForever.IO.Format.WildStar.Shared.Track;

namespace NexusForever.IO.Format.WildStar.M3
{
    // low confidence in this name, more checking needed
    public class Light : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 400u;

        public ushort Unknown0 { get; set; } // possibly bone id
        public ushort Unknown2 { get; set; }
        public short Unknown4 { get; set; }
        public short Unknown6 { get; set; }
        public short Unknown8 { get; set; }
        public ushort UnknownA { get; set; }

        public TrackUIntArray Unknown58 { get; } = new();
        public TrackUIntArray Colour { get; } = new(); // low confidence in this name, more checking needed
        public TrackHalfArray Intensity { get; } = new(); // low confidence in this name, more checking needed
        public TrackUShortArray UnknownA0 { get; } = new();
        public TrackUShortArray UnknownB8 { get; } = new();
        public TrackUShortArray UnknownD0 { get; } = new();
        public TrackUShortArray UnknownE8 { get; } = new();
        public TrackUShortArray Unknown130 { get; } = new();
        public TrackUShortArray Unknown148 { get; } = new();

        public void Read(BinaryReader reader, long dataOffset)
        {
            Unknown0 = reader.ReadUInt16();
            Unknown2 = reader.ReadUInt16();
            Unknown4 = reader.ReadInt16();
            Unknown6 = reader.ReadInt16();
            Unknown8 = reader.ReadInt16();
            UnknownA = reader.ReadUInt16();
            reader.BaseStream.Position += 4; // padding

            // the rest of the structure is 16 tracks, not all have a known value size
            reader.BaseStream.Position += 24 * 3; // tracks 0x10, 0x28 and 0x40, unknown value size

            Unknown58.Read(reader, dataOffset);
            Colour.Read(reader, dataOffset);
            Intensity.Read(reader, dataOffset);
            UnknownA0.Read(reader, dataOffset);
            UnknownB8.Read(reader, dataOffset);
            UnknownD0.Read(reader, dataOffset);
            UnknownE8.Read(reader, dataOffset);

            reader.BaseStream.Position += 24 * 2; // tracks 0x100 and 0x118, unknown value size

            Unknown130.Read(reader, dataOffset);
            Unknown148.Read(reader, dataOffset);

            // tracks 0x160 and 0x178, unknown value size
        }
    }
}
