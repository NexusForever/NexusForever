namespace NexusForever.IO.Format.WildStar.M3
{
    public class CollisionMesh : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 16u;

        public uint Unknown0 { get; set; }
        public uint IndexStart { get; set; }
        public uint IndexCount { get; set; }
        public short UnknownC { get; set; }

        public void Read(BinaryReader reader, long dataOffset)
        {
            Unknown0   = reader.ReadUInt32();
            IndexStart = reader.ReadUInt32();
            IndexCount = reader.ReadUInt32();
            UnknownC   = reader.ReadInt16();
            reader.BaseStream.Position += 2;
        }
    }
}
