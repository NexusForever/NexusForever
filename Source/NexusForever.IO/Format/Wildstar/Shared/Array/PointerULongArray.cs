namespace NexusForever.IO.Format.WildStar.Shared.Array
{
    public class PointerULongArray : PointerArray<ulong>
    {
        protected override uint Size => sizeof(ulong);

        protected override ulong ReadValue(BinaryReader reader, long dataOffset)
        {
            return reader.ReadUInt64();
        }
    }
}
