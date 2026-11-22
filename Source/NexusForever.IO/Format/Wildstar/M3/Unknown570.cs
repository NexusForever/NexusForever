namespace NexusForever.IO.Format.WildStar.M3
{
    public class Unknown570 : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 32u;

        public void Read(BinaryReader reader, long dataOffset)
        {
            reader.ReadBytes((int)Size);
        }
    }
}
