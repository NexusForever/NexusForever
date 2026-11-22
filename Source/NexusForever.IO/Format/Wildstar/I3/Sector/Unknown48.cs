namespace NexusForever.IO.Format.WildStar.I3.Sector
{
    public class Unknown48 : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 20u;

        public void Read(BinaryReader reader, long dataOffset)
        {
            reader.BaseStream.Position += Size;
        }
    }
}
