namespace NexusForever.IO.Format.WildStar.Shared.Track
{
    public abstract class TrackArray<T> : IBinaryReadableOffset
    {
        public uint[] Times { get; private set; }
        public T[] Values { get; private set; }

        public void Read(BinaryReader reader, long dataOffset)
        {
            long count = reader.ReadInt32();
            reader.BaseStream.Position += 4; // padding
            long timeOffset  = reader.ReadInt64();
            long valueOffset = reader.ReadInt64();

            long position = reader.BaseStream.Position;
            reader.BaseStream.Position = dataOffset + timeOffset;

            Times = new uint[count];
            for (uint i = 0u; i < count; i++)
                Times[i] = reader.ReadUInt32();

            reader.BaseStream.Position = dataOffset + valueOffset;

            Values = new T[count];
            for (uint i = 0u; i < count; i++)
                Values[i] = ReadValue(reader, dataOffset + valueOffset);

            reader.BaseStream.Position = position;
        }

        protected abstract uint Size { get; }

        protected abstract T ReadValue(BinaryReader reader, long dataOffset);
    }
}
