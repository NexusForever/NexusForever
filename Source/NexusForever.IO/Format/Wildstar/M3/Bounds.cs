using System.Numerics;

namespace NexusForever.IO.Format.WildStar.M3
{
    public class Bounds : IBinaryReadable
    {
        public Vector3 A { get; set; }
        public Vector3 B { get; set; }
        public Vector3 Centre { get; set; }
        public float Radius { get; set; }

        public void Read(BinaryReader reader)
        {
            A = reader.ReadVector3();
            reader.BaseStream.Position += 4; // padding

            B = reader.ReadVector3();
            reader.BaseStream.Position += 4; // padding

            Centre = reader.ReadVector3();
            reader.BaseStream.Position += 4; // padding

            Radius = reader.ReadSingle();
            reader.BaseStream.Position += 4; // padding
        }
    }
}
