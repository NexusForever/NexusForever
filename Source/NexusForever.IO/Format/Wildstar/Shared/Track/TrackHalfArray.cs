namespace NexusForever.IO.Format.WildStar.Shared.Track
{
    public class TrackHalfArray : TrackArray<float>
    {
        protected override uint Size => sizeof(ushort);

        protected override float ReadValue(BinaryReader reader, long dataOffset)
        {
            return (float)reader.ReadHalf();
        }
    }
}
