namespace NexusForever.IO.Format.WildStar.Shared.Track
{
    public class TrackUShortArray : TrackArray<ushort>
    {
        protected override uint Size => sizeof(ushort);

        protected override ushort ReadValue(BinaryReader reader, long dataOffset)
        {
            return reader.ReadUInt16();
        }
    }
}
