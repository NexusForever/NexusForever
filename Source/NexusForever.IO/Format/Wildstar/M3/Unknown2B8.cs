namespace NexusForever.IO.Format.WildStar.M3
{
    public class Unknown2B8 : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 48u;

        // 8
        // 18

        public void Read(BinaryReader reader, long dataOffset)
        {
            reader.ReadBytes((int)Size);
        }
    }
}
