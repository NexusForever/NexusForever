namespace NexusForever.IO.Format.WildStar.Shared.Array
{
    public abstract class PointerArray<T>
    {
        public T[] Values { get; private set; }

        protected abstract uint Size { get; }

        public void Read(BinaryReader reader, long dataOffset)
        {
            uint count = reader.ReadUInt32();
            reader.BaseStream.Position += 4;
            long offset = reader.ReadInt64();

            long headerPosition = reader.BaseStream.Position;
            long dataPosition   = dataOffset + offset;

            dataOffset += offset + (Size * count + 15 & 0xFFFFFFF0);

            Values = new T[count];
            for (uint i = 0u; i < count; i++)
            {
                reader.BaseStream.Position = dataPosition + i * Size;
                Values[i] = ReadValue(reader, dataOffset);
            }

            reader.BaseStream.Position = headerPosition;
        }

        protected abstract T ReadValue(BinaryReader reader, long dataOffset);
    }
}
