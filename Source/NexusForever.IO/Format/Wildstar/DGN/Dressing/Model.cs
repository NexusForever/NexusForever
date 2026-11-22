using System.Numerics;
using NexusForever.IO.Format.WildStar;
using NexusForever.IO.Format.WildStar.Shared.Array;

namespace NexusForever.IO.Format.Wildstar.DGN.Dressing
{
    public class Model : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 128u;

        public ulong Id { get; set; }
        public PointerStringArray Path { get; } = new();
        public Vector3 Position { get; set; }
        public float Scale { get; set; }
        public Quaternion Rotation { get; set; }
        public PointerUIntArray Unknown60 { get; } = new();
        public PointerReadableArray<Unknown70> Unknown70 { get; } = new();

        public void Read(BinaryReader reader, long dataOffset)
        {
            Id = reader.ReadUInt64();
            Path.Read(reader, dataOffset);

            reader.BaseStream.Position += 24;
            Scale = reader.ReadSingle();
            reader.BaseStream.Position += 12;
            Position = reader.ReadVector3();
            reader.BaseStream.Position += 4;
            Rotation = reader.ReadQuaternion();

            Unknown60.Read(reader, dataOffset);
            Unknown70.Read(reader, dataOffset);
        }
    }
}
