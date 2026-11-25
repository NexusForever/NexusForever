using NexusForever.IO.Format.WildStar.M3.Static;
using NexusForever.IO.Format.WildStar.Shared.Array;

namespace NexusForever.IO.Format.WildStar.M3
{
    public class Texture : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 32u;

        public short Unknown0 { get; set; }
        public TextureType Type { get; set; }
        public int Unknown4 { get; set; }
        public float Unknown8 { get; set; }
        public PointerStringArray Path { get; } = new();

        public void Read(BinaryReader reader, long dataOffset)
        {
            Unknown0 = reader.ReadInt16();
            Type     = (TextureType)reader.ReadByte();
            reader.BaseStream.Position += 1; // padding
            Unknown4 = reader.ReadInt32();
            Unknown8 = reader.ReadSingle();
            reader.BaseStream.Position += 4; // padding

            Path.Read(reader, dataOffset);
        }
    }
}
