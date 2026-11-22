namespace NexusForever.IO.Format.WildStar.M3
{
    public class Unknown328 : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 56u;

        public void Read(BinaryReader reader, long dataOffset)
        {
            reader.ReadBytes((int)Size);
        }
    }
}
