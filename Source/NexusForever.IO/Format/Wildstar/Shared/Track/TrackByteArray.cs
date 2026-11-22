namespace NexusForever.IO.Format.WildStar.Shared.Track
{
    public class TrackByteArray : TrackArray<byte>
    {
        protected override uint Size => sizeof(byte);

        protected override byte ReadValue(BinaryReader reader, long dataOffset)
        {
            return reader.ReadByte();
        }
    }
}
