using System.Numerics;

namespace NexusForever.IO.Format.WildStar.Shared
{
    public class AABB : IBinaryReadable
    {
        public Vector3 Min { get; set; }
        public Vector3 Max { get; set; }

        public void Read(BinaryReader reader)
        {
            Min = reader.ReadVector3();
            reader.ReadSingle();
            Max = reader.ReadVector3();
            reader.ReadSingle();
        }
    }
}
