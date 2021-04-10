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

            if ((cell.Flags & ChnkCellFlags.Zone) != 0)
            {
                Array.Copy(cell.WorldZoneIds, worldZoneIds, 4);
                flags |= Flags.Zone;
            }

            if ((cell.Flags & ChnkCellFlags.HeightMap) != 0)
            {
                for (int y = 0; y < 17; y++)
                    for (int x = 0; x < 17; x++)
                        heightMap[x, y] = (cell.Heightmap[x + 1, y + 1] / 8.0f) - 2048f;

                flags |= Flags.Height;
            }

            if ((cell.Flags & ChnkCellFlags.ZoneBound) != 0)
            {
                for (int y = 0; y < 64; y++)
                    for (int x = 0; x < 64; x++)
                        worldZoneBounds[x, y] = cell.WorldZoneBounds[x, y];

                flags |= Flags.ZoneBound;
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
                    case Flags.Zone:
                    {
                        foreach (uint worldZoneId in worldZoneIds)
                            writer.Write(worldZoneId);
                        break;
                    }
                    case Flags.Height:
                    {
                        for (int y = 0; y < 17; y++)
                            for (int x = 0; x < 17; x++)
                                writer.Write(heightMap[x, y]);
                        break;
                    }
                    case Flags.ZoneBound:
                    {
                        for (int y = 0; y < 64; y++)
                            for (int x = 0; x < 64; x++)
                                writer.Write(worldZoneBounds[x, y]);
                        break;
                    }
                }
            }
        }
    }
}
