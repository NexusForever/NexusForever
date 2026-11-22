namespace NexusForever.IO.Format.WildStar.Shared.Array
{
    public class PointerByteArray : PointerArray<byte>
    {
        protected override uint Size => sizeof(byte);

        protected override byte ReadValue(BinaryReader reader, long dataOffset)
        {
            return reader.ReadByte();
        }
    }
}
