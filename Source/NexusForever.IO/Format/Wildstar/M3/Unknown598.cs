namespace NexusForever.IO.Format.WildStar.M3
{
    public class Unknown598 : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 76u;

        public void Read(BinaryReader reader, long dataOffset)
        {
            reader.ReadBytes((int)Size);
        }
    }
}
