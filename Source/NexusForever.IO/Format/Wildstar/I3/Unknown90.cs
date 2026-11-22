namespace NexusForever.IO.Format.WildStar.I3
{
    public class Unknown90 : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 32u;

        public void Read(BinaryReader reader, long dataOffset)
        {
            reader.BaseStream.Position += Size;
        }
    }
}
