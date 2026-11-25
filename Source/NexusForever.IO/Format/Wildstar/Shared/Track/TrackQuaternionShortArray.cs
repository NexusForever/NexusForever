namespace NexusForever.IO.Format.WildStar.Shared.Track
{
    public class TrackQuaternionShortArray : TrackArray<QuaternionShort>
    {
        protected override uint Size => sizeof(short) * 4;

        protected override QuaternionShort ReadValue(BinaryReader reader, long dataOffset)
        {
            return new QuaternionShort(reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16());
        }
    }
}
