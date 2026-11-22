namespace NexusForever.IO.Format.WildStar.M3
{
    public class CollisionModelSequence : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 4u;

        public ushort ModelSequenceId { get; set; }
        public ushort CollisionIndex { get; set; }

        public void Read(BinaryReader reader, long dataOffset)
        {
            ModelSequenceId = reader.ReadUInt16();
            CollisionIndex  = reader.ReadUInt16();
        }
    }
}
