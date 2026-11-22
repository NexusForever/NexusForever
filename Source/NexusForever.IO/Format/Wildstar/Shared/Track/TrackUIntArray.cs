namespace NexusForever.IO.Format.WildStar.Shared.Track
{
    public class TrackUIntArray : TrackArray<uint>
    {
        protected override uint Size => sizeof(uint);

        protected override uint ReadValue(BinaryReader reader, long dataOffset)
        {
            return reader.ReadUInt32();
        }
    }
}
