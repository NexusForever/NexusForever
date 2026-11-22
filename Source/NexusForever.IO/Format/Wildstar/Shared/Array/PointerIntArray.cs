namespace NexusForever.IO.Format.WildStar.Shared.Array
{
    public class PointerIntArray : PointerArray<int>
    {
        protected override uint Size => sizeof(int);

        protected override int ReadValue(BinaryReader reader, long dataOffset)
        {
            return reader.ReadInt32();
        }
    }
}
