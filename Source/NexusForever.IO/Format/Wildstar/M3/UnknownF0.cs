namespace NexusForever.IO.Format.WildStar.M3
{
    public class UnknownF0 : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 184u;

        public void Read(BinaryReader reader, long dataOffset)
        {
            byte[] s = reader.ReadBytes((int)Size);
            string b = Convert.ToHexString(s);
        }
    }
}
