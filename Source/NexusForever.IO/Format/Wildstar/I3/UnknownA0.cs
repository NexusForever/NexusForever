namespace NexusForever.IO.Format.WildStar.I3
{
    public class UnknownA0 : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 64u;

        public void Read(BinaryReader reader, long dataOffset)
        {
            reader.BaseStream.Position += Size;
        }
    }
}
