namespace NexusForever.IO.Format.WildStar.Shared.Array
{
    public class PointerUShortArray : PointerArray<ushort>
    {
        protected override uint Size => sizeof(ushort);

        protected override ushort ReadValue(BinaryReader reader, long dataOffset)
        {
            return reader.ReadUInt16();
        }
    }
}
