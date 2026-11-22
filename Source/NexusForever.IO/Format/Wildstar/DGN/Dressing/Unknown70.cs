using NexusForever.IO.Format.WildStar;

namespace NexusForever.IO.Format.Wildstar.DGN.Dressing
{
    public class Unknown70 : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 12u;

        public void Read(BinaryReader reader, long dataOffset)
        {
        }
    }
}
