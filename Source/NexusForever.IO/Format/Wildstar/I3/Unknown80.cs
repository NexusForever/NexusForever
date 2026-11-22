namespace NexusForever.IO.Format.WildStar.I3
{
    public class Unknown80 : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 176u;

        public void Read(BinaryReader reader, long dataOffset)
        {
            reader.BaseStream.Position += Size;
        }
    }
}
