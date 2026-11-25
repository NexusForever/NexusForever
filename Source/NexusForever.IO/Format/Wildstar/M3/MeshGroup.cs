namespace NexusForever.IO.Format.WildStar.M3
{
    public class MeshGroup : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 4u;

        public ushort Unknown0 { get; set; }
        public ushort Unknown2 { get; set; }

        public void Read(BinaryReader reader, long dataOffset)
        {
            Unknown0 = reader.ReadUInt16();
            Unknown2 = reader.ReadUInt16();
        }
    }
}
