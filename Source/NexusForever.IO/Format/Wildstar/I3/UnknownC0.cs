namespace NexusForever.IO.Format.WildStar.I3
{
    public class UnknownC0 : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 56u;

        public void Read(BinaryReader reader, long dataOffset)
        {
            reader.BaseStream.Position += Size;
        }
    }
}
