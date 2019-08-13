using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using NexusForever.Shared.Game.Map;

namespace NexusForever.Shared.IO.Map
{
    public class MapFile : IReadable, IEnumerable<MapFileGrid>
    {
        protected const uint Magic   = 0x504D464Eu; // NFMP
        protected const uint Version = 2u;
        protected const uint Build   = 16042u;

        public string Asset { get; protected set; }

        protected readonly Dictionary<uint /*hash*/, MapFileGrid> grids = new Dictionary<uint, MapFileGrid>();

        private static (uint gridX, uint gridY) GetGridCoord(Vector3 vector)
        {
            int gridX = MapDefines.WorldGridOrigin + (int)Math.Floor(vector.X / MapDefines.GridSize);
            if (gridX < 0 || gridX > MapDefines.WorldGridCount)
                throw new ArgumentOutOfRangeException($"Position X: {vector.X} is invalid!");

            int gridZ = MapDefines.WorldGridOrigin + (int)Math.Floor(vector.Z / MapDefines.GridSize);
            if (gridZ < 0 || gridZ > MapDefines.WorldGridCount)
                throw new ArgumentOutOfRangeException($"Position Z: {vector.Z} is invalid!");

            return ((uint)gridX, (uint)gridZ);
        }

        public void Read(BinaryReader reader)
        {
            ReadHeader(reader);
            Asset = reader.ReadString();

            uint gridCount = reader.ReadUInt32();
            for (int i = 0; i < gridCount; i++)
            {
                var grid = new MapFileGrid();
                grid.Read(reader);
                grids.Add((grid.X << 16) | grid.Y, grid);
            }

            if (reader.BaseStream.Position != reader.BaseStream.Length)
                throw new InvalidDataException();
        }

        public void ReadHeader(BinaryReader reader)
        {
            if (reader.ReadUInt32() != Magic)
                throw new InvalidDataException();

            if (reader.ReadUInt32() != Version)
                throw new InvalidDataException("Invalid map version, use the MapGenerator to create the latest map files.");

            if (reader.ReadUInt32() != Build)
                throw new InvalidDataException("Invalid client build, use the MapGenerator to create map files from the correct client build.");
        }

        /// <summary>
        /// Return world area id at supplied position.
        /// </summary>
        public uint GetWorldAreaId(Vector3 vector)
        {
            MapFileGrid grid = GetGrid(vector);
            if (grid == null)
                return 0u;

            return grid.GetWorldAreaId(vector);
        }

        /// <summary>
        /// Return terrain height at supplied position.
        /// </summary>
        public float GetTerrainHeight(Vector3 vector)
        {
            MapFileGrid grid = GetGrid(vector);
            if (grid == null)
                return 0f;

            return grid.GetTerrainHeight(vector);
        }

        private MapFileGrid GetGrid(Vector3 vector)
        {
            (uint gridX, uint gridY) = GetGridCoord(vector);
            uint hash = (gridY << 16) | gridX;
            return grids.TryGetValue(hash, out MapFileGrid grid) ? grid : null;
        }

        public IEnumerator<MapFileGrid> GetEnumerator()
        {
            return grids.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
