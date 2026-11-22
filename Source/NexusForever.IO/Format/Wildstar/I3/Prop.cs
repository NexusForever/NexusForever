using System.Numerics;

namespace NexusForever.IO.Format.WildStar.I3
{
    public class Prop : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 64u;

        public uint PropNameIndex { get; set; }
        public uint PathNameIndex { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }
        public Quaternion Rotation { get; set; }

        public void Read(BinaryReader reader, long dataOffset)
        {
            PropNameIndex = reader.ReadUInt32();
            PathNameIndex = reader.ReadUInt32();
            reader.BaseStream.Position += 8;

            Position = reader.ReadVector3();
            reader.BaseStream.Position += 4;

            Scale = reader.ReadVector3();
            reader.BaseStream.Position += 4;

            Rotation = reader.ReadQuaternion();
        }
    }
}
