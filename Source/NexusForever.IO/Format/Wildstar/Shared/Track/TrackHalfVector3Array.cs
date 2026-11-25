using System.Numerics;

namespace NexusForever.IO.Format.WildStar.Shared.Track
{
    public class TrackHalfVector3Array : TrackArray<Vector3>
    {
        protected override uint Size => sizeof(ushort) * 3;

        protected override Vector3 ReadValue(BinaryReader reader, long dataOffset)
        {
            return new Vector3((float)reader.ReadHalf(), (float)reader.ReadHalf(), (float)reader.ReadHalf());
        }
    }
}
