using NexusForever.IO.Format.WildStar.Shared.Array;

namespace NexusForever.IO.Format.WildStar.I3.Sector
{
    public class I3SectorFile : IBinaryReadable
    {
        // client hard codes this length
        public static uint DataOffset => 432u;

        public PointerStringArray Name { get; set; } = new();
        public PointerVector3Array Vertices { get; set; } = new();
        public PointerReadableArray<Unknown48> Unknown48 { get; set; } = new();
        public PointerUIntArray Indices { get; set; } = new();
        public PointerUShortArray Unknown68 { get; set; } = new();
        public PointerReadableArray<Unknown78> Unknown78 { get; set; } = new();

        public void Read(BinaryReader reader)
        {
            uint magic = reader.ReadUInt32();
            if (magic != 0x53444546)
                throw new InvalidDataException();

            uint version = reader.ReadUInt32();
            if (version != 30)
                throw new InvalidDataException();

            reader.BaseStream.Position += 24;
            Name.Read(reader, DataOffset);

            reader.BaseStream.Position += 8;

            Vertices.Read(reader, DataOffset);
            Unknown48.Read(reader, DataOffset);
            Indices.Read(reader, DataOffset);
            Unknown68.Read(reader, DataOffset);
            Unknown78.Read(reader, DataOffset);
            // 88
            // B8
        }
    }
}
