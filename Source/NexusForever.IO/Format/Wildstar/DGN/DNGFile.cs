using NexusForever.IO.Format.WildStar.Shared.Array;

namespace NexusForever.IO.Format.Wildstar.DGN
{
    public class DNGFile : IBinaryReadable
    {
        // client hard codes this length
        public static uint DataOffset => 80u;

        public PointerStringArray Guid { get; } = new();
        public PointerStringArray Name { get; } = new();
        public PointerStringArray I3File { get; } = new();
        public PointerReadableArray<DressingInfo> Dressings { get; } = new();

        public void Read(BinaryReader reader)
        {
            uint magic = reader.ReadUInt32();
            if (magic != 0x444E474E)
                throw new InvalidDataException();

            uint version = reader.ReadUInt32();
            if (version != 6)
                throw new InvalidDataException();

            Guid.Read(reader, DataOffset);
            Name.Read(reader, DataOffset);
            I3File.Read(reader, DataOffset);
            Dressings.Read(reader, DataOffset);
        }
    }
}
