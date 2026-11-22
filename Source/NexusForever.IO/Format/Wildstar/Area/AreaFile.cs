namespace NexusForever.IO.Format.WildStar.Area
{
    public class AreaFile : IBinaryReadable
    {
        public HashSet<IBinaryReadable> Chunks { get; } = [];

        public void Read(BinaryReader reader)
        {
            uint magic = reader.ReadUInt32();
            if (magic != 0x61726561 && magic != 0x41524541)
                throw new InvalidDataException();

            reader.ReadUInt32();

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                uint chunkMagic  = reader.ReadUInt32();
                uint chunkSize   = reader.ReadUInt32();
                byte[] chunkData = reader.ReadBytes((int)chunkSize);

                IBinaryReadable chunk = chunkMagic switch
                {
                    0x44484D4F => new DhmoChunk(),
                    0x43484E4B => new CellChunk(),
                    0x43555254 => new CurtChunk(),
                    0x50524F70 => new PropChunk(),
                    _          => throw new NotImplementedException(),
                };

                using var chunkStream = new MemoryStream(chunkData);
                using var chunkReader = new BinaryReader(chunkStream);
                chunk.Read(chunkReader);
                Chunks.Add(chunk);
            }
        }
    }
}
