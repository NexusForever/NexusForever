using NexusForever.IO.Format.WildStar.Area.Static;

namespace NexusForever.IO.Format.WildStar.Area
{
    public class Cell : IBinaryReadable
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

        public uint X { get; set; }
        public uint Y { get; set; }
        public ChnkCellFlags Flags { get; set; }
        public uint[] WorldZoneIds { get; set; }
        public byte[,] WorldZoneBounds { get; set; }
        public ushort[,] Heightmap { get; set; }

        public HashSet<IBinaryReadable> Chunks { get; } = [];

        public void Read(BinaryReader reader)
        {
            Flags = (ChnkCellFlags)reader.ReadUInt32();

            for (int i = 0; i < 32; i++)
            {
                ChnkCellFlags flag = (ChnkCellFlags)(1 << i);
                if ((Flags & flag) == 0)
                    continue;

                switch (flag)
                {
                    case ChnkCellFlags.Zone:
                    {
                        WorldZoneIds = new uint[4];
                        for (int j = 0; j < 4; j++)
                            WorldZoneIds[j] = reader.ReadUInt32();
                        break;
                    }
                    case ChnkCellFlags.HeightMap:
                    {
                        Heightmap = new ushort[19, 19];
                        for (int y = 0; y < 19; y++)
                            for (int x = 0; x < 19; x++)
                                Heightmap[x, y] = reader.ReadUInt16();
                        break;
                    }
                    case ChnkCellFlags.ZoneBound:
                    {
                        WorldZoneBounds = new byte[64, 64];
                        for (int y = 0; y < 64; y++)
                            for (int x = 0; x < 64; x++)
                                WorldZoneBounds[x, y] = reader.ReadByte();
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

                IBinaryReadable chunk = chunkMagic switch
                {
                    0x77627350 => new WbspChunk(),
                    0x63757244 => new CurdChunk(),
                    0x50524F50 => new CellPropChunk(),
                    0x57417447 => new WatgChunk(),
                    _          => throw new InvalidDataException(),
                };

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
