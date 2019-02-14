using System;
using System.IO;
using System.Linq;
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

            if (cell.WorldAreaIds.Any(a => a != 0u))
            {
                Array.Copy(cell.WorldAreaIds, worldAreaIds, 4);
                flags |= Flags.Area;
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
                }
            }
        }
    }
}
