using System;
using System.IO;
using NexusForever.MapGenerator.IO.Area;
using NexusForever.Shared.IO;
using NexusForever.Shared.IO.Map;

namespace NexusForever.MapGenerator.IO.Map
{
    public class WritableMapFileCell : MapFileCell, IWritable
    {
        public WritableMapFileCell(ChnkCell cell)
        {
            X = cell.X;
            Y = cell.Y;

            if ((cell.Flags & ChnkCellFlags.Area) != 0)
            {
                Array.Copy(cell.WorldAreaIds, worldAreaIds, 4);
                flags |= Flags.Area;
            }

            if ((cell.Flags & ChnkCellFlags.HeightMap) != 0)
            {
                for (int y = 0; y < 17; y++)
                    for (int x = 0; x < 17; x++)
                        heightMap[x, y] = (cell.Heightmap[x + 1, y + 1] / 8.0f) - 2048f;

                flags |= Flags.Height;
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
            writer.Write((uint)flags);

            for (int i = 0; i < 32; i++)
            {
                Flags flag = (Flags)(1 << i);
                if ((flags & flag) == 0)
                    continue;

                switch (flag)
                {
                    case Flags.Area:
                    {
                        foreach (uint worldAreaId in worldAreaIds)
                            writer.Write(worldAreaId);
                        break;
                    }
                    case Flags.Height:
                    {
                        for (int y = 0; y < 17; y++)
                            for (int x = 0; x < 17; x++)
                                writer.Write(heightMap[x, y]);
                        break;
                    }
                }
            }
        }
    }
}
