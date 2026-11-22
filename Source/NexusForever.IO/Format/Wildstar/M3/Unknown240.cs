namespace NexusForever.IO.Format.WildStar.M3
{
    public class Unknown240 : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 112u;

        public void Read(BinaryReader reader, long dataOffset)
        {
            reader.ReadBytes((int)Size);
        }
    }
}
