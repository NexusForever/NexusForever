namespace NexusForever.IO.Format.WildStar.Shared.Array
{
    public class PointerStringArray : PointerArray<char>
    {
        protected override uint Size => sizeof(char);

        public string Value => Values?.Length > 0 ? new(Values, 0, Values.Length - 1) : null;

        protected override char ReadValue(BinaryReader reader, long dataOffset)
        {
            return Convert.ToChar(reader.ReadUInt16());
        }
    }
}
