namespace NexusForever.IO.Format.WildStar.M3
{
    public class Unknown230 : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 4u;

        public void Read(BinaryReader reader, long dataOffset)
        {
            reader.ReadBytes((int)Size);
        }
    }
}
