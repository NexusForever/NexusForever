using System.Numerics;

namespace NexusForever.IO.Format.WildStar.Shared.Track
{
    public class TrackVector3Array : TrackArray<Vector3>
    {
        protected override uint Size => sizeof(float) * 3;

        protected override Vector3 ReadValue(BinaryReader reader, long dataOffset)
        {
            return reader.ReadVector3();
        }
    }
}
