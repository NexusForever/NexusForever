namespace NexusForever.IO.Format.WildStar.M3
{
    public class Unknown80 : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 48u;

        public void Read(BinaryReader reader, long dataOffset)
        {
            reader.ReadBytes((int)Size);
        }
    }
}
