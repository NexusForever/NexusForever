namespace NexusForever.IO.Format.WildStar.M3
{
    public class Unknown2F8 : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 160u;

        public void Read(BinaryReader reader, long dataOffset)
        {
            reader.ReadBytes((int)Size);
        }
    }
}
