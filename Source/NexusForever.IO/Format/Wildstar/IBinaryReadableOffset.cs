namespace NexusForever.IO.Format.WildStar
{
    public interface IBinaryReadableOffset
    {
        void Read(BinaryReader reader, long dataOffset);
    }
}
