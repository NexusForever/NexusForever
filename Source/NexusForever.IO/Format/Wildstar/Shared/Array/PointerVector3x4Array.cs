using System.Numerics;

namespace NexusForever.IO.Format.WildStar.Shared.Array
{
    public class PointerVector3x4Array : PointerArray<Vector3>
    {
        protected override uint Size => sizeof(float) * 4;

        protected override Vector3 ReadValue(BinaryReader reader, long dataOffset)
        {
            var v = reader.ReadVector3();
            reader.ReadSingle();
            return v;
        }
    }
}
