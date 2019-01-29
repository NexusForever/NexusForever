using System.IO;
using NexusForever.Shared.IO;
using NexusForever.Shared.IO.Map;

namespace NexusForever.MapGenerator.IO.Map
{
    public class WritableMapFile : MapFile, IWritable
    {
        public WritableMapFile(string asset)
        {
            Asset = asset;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Magic);
            writer.Write(Version);
            writer.Write(Build);
            writer.Write(Asset);

            writer.Write(grids.Count);
            foreach (WritableMapFileGrid grid in grids.Values)
                grid.Write(writer);
        }

        public void SetGrid(uint x, uint y, WritableMapFileGrid grid)
        {
            grids.Add((grid.X << 16) | grid.Y, grid);
        }
    }
}
