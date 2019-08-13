using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using NexusForever.Shared.Game.Map;

namespace NexusForever.Shared.IO.Map
{
    public class MapFileGrid : IReadable, IEnumerable<MapFileCell>
    {
        public uint X { get; protected set; }
        public uint Y { get; protected set; }

        protected readonly Dictionary<uint /*hash*/, MapFileCell> cells = new Dictionary<uint, MapFileCell>();

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
        /// Return world area id at supplied position.
        /// </summary>
        public uint GetWorldAreaId(Vector3 vector)
        {
            MapFileCell cell = GetCell(vector);
            return cell?.GetWorldAreaIds()?.FirstOrDefault(worldAreaId => worldAreaId != 0u) ?? 0u;
        }

        /// <summary>
        /// Return terrain height at supplied position.
        /// </summary>
        public float GetTerrainHeight(Vector3 vector)
        {
            MapFileCell cell = GetCell(vector);
            return cell?.GetTerrainHeight(vector) ?? 0f;
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
