﻿using System.Collections;
using System.Numerics;

namespace NexusForever.IO.Map
{
    public class MapFileGrid : IReadable, IEnumerable<MapFileCell>
    {
        public uint X { get; protected set; }
        public uint Y { get; protected set; }

        protected readonly Dictionary<uint /*hash*/, MapFileCell> cells = new();

        private static (uint cellX, uint cellY) GetCellCoord(Vector3 vector)
        {
            int x = (int)Math.Floor(MapDefines.GridCellCount * (MapDefines.WorldGridOrigin + vector.X / MapDefines.GridSize));
            if (x < 0 || x > MapDefines.WorldGridCount * MapDefines.GridCellCount)
                throw new ArgumentOutOfRangeException($"Position X: {vector.X} is invalid!");

            int z = (int)Math.Floor(MapDefines.GridCellCount * (MapDefines.WorldGridOrigin + vector.Z / MapDefines.GridSize));
            if (z < 0 || z > MapDefines.WorldGridCount * MapDefines.GridCellCount)
                throw new ArgumentOutOfRangeException($"Position Z: {vector.X} is invalid!");

            return ((uint)x & MapDefines.GridCellCount - 1, (uint)z & MapDefines.GridCellCount - 1);
        }

        public void Read(BinaryReader reader)
        {
            X = reader.ReadUInt32();
            Y = reader.ReadUInt32();

            uint cellCount = reader.ReadUInt32();
            for (int i = 0; i < cellCount; i++)
            {
                var cell = new MapFileCell();
                cell.Read(reader);
                cells.Add((cell.X << 16) | cell.Y, cell);
            }
        }

        /// <summary>
        /// Return world zone id at supplied position.
        /// </summary>
        public uint? GetWorldZoneId(Vector3 vector)
        {
            MapFileCell cell = GetCell(vector);
            return cell?.GetWorldZoneId(vector);
        }

        /// <summary>
        /// Return terrain height at supplied position.
        /// </summary>
        public float? GetTerrainHeight(Vector3 vector)
        {
            MapFileCell cell = GetCell(vector);
            return cell?.GetTerrainHeight(vector);
        }

        private MapFileCell GetCell(Vector3 vector)
        {
            (uint cellX, uint cellY) = GetCellCoord(vector);
            uint hash = (cellX << 16) | cellY;
            return cells.TryGetValue(hash, out MapFileCell cell) ? cell : null;
        }

        public IEnumerator<MapFileCell> GetEnumerator()
        {
            return cells.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
