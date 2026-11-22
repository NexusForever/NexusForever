using NexusForever.IO.Format.WildStar.Shared;

namespace NexusForever.IO.Format.WildStar.I3
{
    public class SectorInfo : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 64u;

        public uint NameIndex { get; set; }
        public AABB BoundingBox { get; set; } = new();

        public void Read(BinaryReader reader, long dataOffset)
        {
            var ss = reader.ReadBytes((int)Size);
            var s = Convert.ToHexString(ss);
            reader.BaseStream.Position -= Size;

            NameIndex = reader.ReadUInt32();
            reader.BaseStream.Position += 12;

            BoundingBox.Read(reader);
            reader.BaseStream.Position += 16;
        }
    }
}
