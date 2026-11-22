using System.Numerics;

namespace NexusForever.IO.Format.WildStar.M3
{
    public class Unknown510 : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 16u;

        public Quaternion Unknown0 { get; set; }

        public void Read(BinaryReader reader, long dataOffset)
        {
            Unknown0 = reader.ReadQuaternion();
        }
    }
}
