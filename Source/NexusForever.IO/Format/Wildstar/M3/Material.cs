using NexusForever.IO.Format.WildStar.Shared.Array;

namespace NexusForever.IO.Format.WildStar.M3
{
    public class Material : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 48u;

        public byte[] Unknown0 { get; set; } = new byte[12];
        public ushort UnknownC { get; set; }
        public uint Unknown10 { get; set; }
        public uint Unknown14 { get; set; }
        public int Unknown18 { get; set; }
        public int Unknown1C { get; set; }
        public PointerReadableArray<MaterialDescription> Descriptions { get; } = new();

        public void Read(BinaryReader reader, long dataOffset)
        {
            Unknown0  = reader.ReadBytes(12);
            UnknownC  = reader.ReadUInt16();
            reader.BaseStream.Position += 2; // padding
            Unknown10 = reader.ReadUInt32();
            Unknown14 = reader.ReadUInt32();
            Unknown18 = reader.ReadInt32();
            Unknown1C = reader.ReadInt32();

            Descriptions.Read(reader, dataOffset);
        }
    }
}
