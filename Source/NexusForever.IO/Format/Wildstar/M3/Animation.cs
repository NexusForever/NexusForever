namespace NexusForever.IO.Format.WildStar.M3
{
    public class Animation : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 112u;

        public ushort ModelSequenceId { get; set; }
        public ushort FallbackModelSequenceId { get; set; }
        public uint Start { get;set; }
        public uint End { get;set; }

        public void Read(BinaryReader reader, long dataOffset)
        {
            ModelSequenceId = reader.ReadUInt16();
            reader.ReadUInt16();
            reader.ReadUInt16(); // 1
            reader.ReadUInt16();
            reader.ReadUInt16(); // 100
            FallbackModelSequenceId = reader.ReadUInt16();
            Start = reader.ReadUInt32();
            End = reader.ReadUInt32();

            reader.ReadUInt16();
            reader.ReadUInt16();
            reader.ReadUInt16();
            reader.ReadUInt16();
            reader.ReadUInt16();
            reader.ReadUInt16();

            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadUInt16();
            reader.BaseStream.Position += 2; // padding

            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadUInt16();
            reader.BaseStream.Position += 2; // padding

            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadUInt16();
            reader.BaseStream.Position += 2; // padding

            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadUInt16();
            reader.BaseStream.Position += 2; // padding

            reader.ReadUInt64(); // 0
            reader.ReadUInt64(); // 0

        }
    }
}
