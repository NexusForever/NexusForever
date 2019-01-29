using System;
using System.Collections.Generic;
using System.IO;
using NexusForever.Shared.IO;

namespace NexusForever.MapGenerator.IO.Area
{
    public class AreaFile
    {
        public HashSet<IReadable> Chunks { get; } = new HashSet<IReadable>();

        public AreaFile(Stream stream)
        {
            using (var reader = new BinaryReader(stream))
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

                    IReadable chunk;
                    switch (chunkMagic)
                    {
                        case 0x44484D4F:
                            chunk = new Dhmo();
                            break;
                        case 0x43484E4B:
                            chunk = new Chnk();
                            break;
                        case 0x43555254:
                            chunk = new Curt();
                            break;
                        case 0x50524F70:
                            chunk = new Prop();
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    using (var chunkStream = new MemoryStream(chunkData))
                    using (var chunkReader = new BinaryReader(chunkStream))
                    {
                        chunk.Read(chunkReader);
                    }

                    Chunks.Add(chunk);
                }
            }
        }
    }
}
