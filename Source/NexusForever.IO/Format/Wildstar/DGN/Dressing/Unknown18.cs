using NexusForever.IO.Format.WildStar;

namespace NexusForever.IO.Format.Wildstar.DGN.Dressing
{
    public class Unknown18 : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 20u;

        public void Read(BinaryReader reader, long dataOffset)
        {
        }
    }
}
