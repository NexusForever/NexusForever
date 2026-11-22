namespace NexusForever.IO.Format.WildStar.Shared.Array
{
    public class PointerSingleArray : PointerArray<float>
    {
        protected override uint Size => sizeof(uint);

        protected override float ReadValue(BinaryReader reader, long dataOffset)
        {
            return reader.ReadSingle();
        }
    }
}
