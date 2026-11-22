namespace NexusForever.IO.Format.WildStar.M3
{
    public class CollisionUnknown : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 20u;

        public float Unknown0 { get; set; }
        public uint Unknown4 { get; set; }
        public uint Unknown8 { get; set; }
        public uint UnknownC { get; set; }
        public uint Unknown10 { get; set; }

        public void Read(BinaryReader reader, long dataOffset)
        {
            Unknown0  = reader.ReadSingle();
            Unknown4  = reader.ReadUInt32();
            Unknown8  = reader.ReadUInt32();
            UnknownC  = reader.ReadUInt32();
            Unknown10 = reader.ReadUInt32();
        }
    }
}
