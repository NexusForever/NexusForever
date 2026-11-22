using System.Numerics;
using NexusForever.IO.Format.WildStar.Shared;
using NexusForever.IO.Format.WildStar.Shared.Array;

namespace NexusForever.IO.Format.WildStar.I3
{
    public class Portal : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 144u;

        public uint NameIndex { get; set; }
        public ushort SectorIndexFrom { get; set; }
        public ushort SectorIndexTo { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public PointerVector3x4Array Vertices { get; } = new();
        public PointerUShortArray Indices { get; } = new();
        public AABB Bounds { get; set; } = new();
        public float Side { get; set; }

        public void Read(BinaryReader reader, long dataOffset)
        {
            NameIndex = reader.ReadUInt32();
            SectorIndexFrom = reader.ReadUInt16();
            SectorIndexTo = reader.ReadUInt16();

            reader.BaseStream.Position += 8;

            Position = reader.ReadVector3();
            reader.BaseStream.Position += 20;

            Rotation = reader.ReadQuaternion();
            Vertices.Read(reader, dataOffset);
            Indices.Read(reader, dataOffset);
            Bounds.Read(reader);

            reader.BaseStream.Position += 8;

            Side = reader.ReadSingle();
        }
    }
}
