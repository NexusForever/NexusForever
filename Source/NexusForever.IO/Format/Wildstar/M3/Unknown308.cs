namespace NexusForever.IO.Format.WildStar.M3
{
    public class Unknown308 : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 80u;

        public void Read(BinaryReader reader, long dataOffset)
        {
            reader.ReadBytes((int)Size);
        }
    }
}
