namespace NexusForever.IO.Format.WildStar.M3
{
    public class Unknown318 : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 400u;

        public void Read(BinaryReader reader, long dataOffset)
        {
            // lots of sub structures
            reader.ReadBytes((int)Size);
        }
    }
}
