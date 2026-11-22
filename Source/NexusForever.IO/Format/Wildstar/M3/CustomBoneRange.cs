using System.Numerics;

namespace NexusForever.IO.Format.WildStar.M3
{
    public class CustomBoneRange : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 76u;

        public ushort BoneId { get; set; }
        public Vector3 Unknown4 { get; set; }  // possibly translation minimum
        public Vector3 Unknown10 { get; set; } // possibly translation maximum
        public Vector3 Unknown1C { get; set; } // possibly rotation minimum
        public Vector3 Unknown28 { get; set; } // possibly rotation maximum
        public Vector3 ScaleMin { get; set; }
        public Vector3 ScaleMax { get; set; }

        public void Read(BinaryReader reader, long dataOffset)
        {
            BoneId    = reader.ReadUInt16();
            reader.BaseStream.Position += 2; // padding
            Unknown4  = reader.ReadVector3();
            Unknown10 = reader.ReadVector3();
            Unknown1C = reader.ReadVector3();
            Unknown28 = reader.ReadVector3();
            ScaleMin  = reader.ReadVector3();
            ScaleMax  = reader.ReadVector3();
        }
    }
}
