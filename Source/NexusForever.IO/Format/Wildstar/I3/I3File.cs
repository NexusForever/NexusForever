using NexusForever.IO.Format.WildStar.Shared;
using NexusForever.IO.Format.WildStar.Shared.Array;

namespace NexusForever.IO.Format.WildStar.I3
{
    public class I3File : IBinaryReadable
    {
        // client hard codes this length
        public static uint DataOffset => 368u;

        public PointerReadableArray<SectorInfo> Sectors { get; set; } = new();
        public PointerReadableArray<Portal> Portals { get; set; } = new();
        public PointerReadableArray<Pass> Passes { get; set; } = new();
        public PointerUShortArray Unknown50 { get; set; } = new();
        public PointerReadableArray<Prop> Props { get; set; } = new();
        public PointerReadableArray<Unknown70> Unknown70 { get; set; } = new();
        public PointerReadableArray<Unknown80> Unknown80 { get; set; } = new();
        public PointerReadableArray<Unknown90> Unknown90 { get; set; } = new();
        public PointerReadableArray<UnknownA0> UnknownA0 { get; set; } = new();
        public PointerUShortArray UnknownB0 { get; set; } = new();
        public PointerReadableArray<UnknownC0> UnknownC0 { get; set; } = new();
        public PointerReadableArray<UnknownD0> UnknownD0 { get; set; } = new();
        public PointerStringArray UnknownE0 { get; set; } = new();
        public PointerStringArray UnknownF0 { get; set; } = new();
        public PointerUIntArray Unknown100 { get; set; } = new();
        public PointerUIntArray Unknown110 { get; set; } = new();
        public PointerStringArray NameData { get; set; } = new();
        public PointerUIntArray Unknown130 { get; set; } = new();
        public AABB Bounds { get; set; } = new();

        public void Read(BinaryReader reader)
        {
            uint magic = reader.ReadUInt32();
            if (magic != 0x49444546)
                throw new InvalidDataException("I3 file magic mismatch!");

            uint version = reader.ReadUInt32();
            if (version != 41)
                throw new InvalidDataException();

            reader.BaseStream.Position += 24;

            Sectors.Read(reader, DataOffset);
            Portals.Read(reader, DataOffset);
            Passes.Read(reader, DataOffset);
            Unknown50.Read(reader, DataOffset);
            Props.Read(reader, DataOffset);
            Unknown70.Read(reader, DataOffset);
            Unknown80.Read(reader, DataOffset);
            Unknown90.Read(reader, DataOffset);
            UnknownA0.Read(reader, DataOffset);
            UnknownB0.Read(reader, DataOffset);
            UnknownC0.Read(reader, DataOffset);
            UnknownD0.Read(reader, DataOffset);
            UnknownE0.Read(reader, DataOffset);
            UnknownF0.Read(reader, DataOffset);
            Unknown100.Read(reader, DataOffset);
            Unknown110.Read(reader, DataOffset);
            NameData.Read(reader, DataOffset);
            Unknown130.Read(reader, DataOffset);
            Bounds.Read(reader);

            reader.BaseStream.Position += 16;
        }
    }
}
