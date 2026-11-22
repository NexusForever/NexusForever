namespace NexusForever.IO.Format.WildStar.M3
{
    public class Unknown2A8 : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 16u;

        public void Read(BinaryReader reader, long dataOffset)
        {
            reader.ReadBytes((int)Size);
        }
    }
}
