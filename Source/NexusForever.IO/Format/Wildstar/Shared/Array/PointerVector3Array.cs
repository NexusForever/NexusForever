using System.Numerics;

namespace NexusForever.IO.Format.WildStar.Shared.Array
{
    public class PointerVector3Array : PointerArray<Vector3>
    {
        protected override uint Size => sizeof(float) * 3;

        protected override Vector3 ReadValue(BinaryReader reader, long dataOffset)
        {
            return reader.ReadVector3();
        }
    }
}
