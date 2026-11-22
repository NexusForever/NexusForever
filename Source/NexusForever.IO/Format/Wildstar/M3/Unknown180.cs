namespace NexusForever.IO.Format.WildStar.M3
{
    public class Unknown180 : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 352u;

        public void Read(BinaryReader reader, long dataOffset)
        {
            reader.ReadBytes((int)Size);
        }
    }
}
