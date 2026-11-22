namespace NexusForever.IO.Format.WildStar.I3
{
    public class UnknownD0 : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 8u;

        public void Read(BinaryReader reader, long dataOffset)
        {
            reader.BaseStream.Position += Size;
        }
    }
}
