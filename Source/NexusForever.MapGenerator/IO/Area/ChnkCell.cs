using System;
using System.Collections.Generic;
using System.IO;
using NexusForever.Shared.IO;

namespace NexusForever.MapGenerator.IO.Area
{
    public class ChnkCell : IReadable
    {
        // data size for each flag
        private static readonly int[] dataSize =
        {
            722,
            16,
            8450,
            8450,
            8450,
            4,
            64,
            16,
            4225,
            2178,
            4,
            578,
            1,
            4624,
            2312,
            8450,
            4096,
            2312,
            2312,
            2312,
            2312,
            1,
            16,
            16900,
            8,
            8450,
            21316,
            4096,
            16,
            8450,
            8450,
            2312
        };

        [Flags]
        public enum Flags
        {
            HeightMap = 0x00000001,
            Area      = 0x10000000
        }

        public uint X { get; }
        public uint Y { get; }

        public uint[] WorldAreaIds = new uint[4];
        public HashSet<IReadable> Chunks { get; } = new HashSet<IReadable>();

        public ChnkCell(uint x, uint y)
        {
            X = x;
            Y = y;
        }

        public void Read(BinaryReader reader)
        {
            Flags flags = (Flags)reader.ReadUInt32();

            for (int i = 0; i < 32; i++)
            {
                Flags flag = (Flags)(1 << i);
                if ((flags & flag) == 0)
                    continue;

                switch (flag)
                {
                    case Flags.Area:
                    {
                        for (int j = 0; j < 4; j++)
                            WorldAreaIds[j] = reader.ReadUInt32();
                        break;
                    }
                    // unhandled flag, skip data
                    default:
                        reader.ReadBytes(dataSize[i]);
                        break;
                }
            }

            // cells can have trailing sub chunks
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                uint chunkMagic  = reader.ReadUInt32();
                uint chunkSize   = reader.ReadUInt32();
                byte[] chunkData = reader.ReadBytes((int)chunkSize);

                IReadable chunk;
                switch (chunkMagic)
                {
                    case 0x77627350:
                        chunk = new Wbsp();
                        break;
                    case 0x63757244:
                        chunk = new Curd();
                        break;
                    case 0x50524F50:
                        chunk = new CellProp();
                        break;
                    case 0x57417447:
                        chunk = new Watg();
                        break;
                    default:
                        throw new InvalidDataException();
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
