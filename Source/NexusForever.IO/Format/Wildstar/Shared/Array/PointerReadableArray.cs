namespace NexusForever.IO.Format.WildStar.Shared.Array
{
    public class PointerReadableArray<T> : PointerArray<T> where T : IStaticSize, IBinaryReadableOffset, new()
    {
        protected override uint Size => T.Size;

        protected override T ReadValue(BinaryReader reader, long dataOffset)
        {
            var value = new T();
            value.Read(reader, dataOffset);
            return value;
        }
    }
}
