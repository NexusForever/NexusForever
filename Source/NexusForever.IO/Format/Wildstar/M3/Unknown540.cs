namespace NexusForever.IO.Format.WildStar.M3
{
    public class Unknown540 : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 112u;

        public void Read(BinaryReader reader, long dataOffset)
        {
            reader.ReadBytes((int)Size);
        }
    }
}
