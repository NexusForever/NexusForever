namespace NexusForever.IO.Format.WildStar.Shared.Array
{
    public class PointerUIntArray : PointerArray<uint>
    {
        protected override uint Size => sizeof(uint);

        protected override uint ReadValue(BinaryReader reader, long dataOffset)
        {
            return reader.ReadUInt32();
        }
    }
}
