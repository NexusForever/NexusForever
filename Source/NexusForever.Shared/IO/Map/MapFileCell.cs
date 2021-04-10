using System;
using System.IO;
using System.Numerics;
using NexusForever.Shared.Game.Map;
using NexusForever.Shared.IO.Map.Static;

namespace NexusForever.Shared.IO.Map
{
    public class MapFileCell : IReadable
    {
        [Flags]
        public enum Flags
        {
            None      = 0x00,
            Zone      = 0x01,
            Height    = 0x02,
            Aura      = 0x04,
            Liquid    = 0x08,
            ZoneBound = 0x10
        }

        public uint X { get; protected set; }
        public uint Y { get; protected set; }

        protected Flags flags;

        protected readonly uint[] worldZoneIds = new uint[4];
        protected readonly byte[,] worldZoneBounds = new byte[64, 64];
        protected readonly float[,] heightMap = new float[17, 17];

        public void Read(BinaryReader reader)
        {
            X     = reader.ReadUInt32();
            Y     = reader.ReadUInt32();
            flags = (Flags)reader.ReadUInt32();

            for (int i = 0; i < 32; i++)
            {
                Flags flag = (Flags)(1 << i);
                if ((flags & flag) == 0)
                    continue;

                switch (flag)
                {
                    case Flags.Zone:
                    {
                        for (int j = 0; j < worldZoneIds.Length; j++)
                            worldZoneIds[j] = reader.ReadUInt32();
                        break;
                    }
                    case Flags.Height:
                    {
                        for (int y = 0; y < 17; y++)
                            for (int x = 0; x < 17; x++)
                                heightMap[x, y] = reader.ReadSingle();
                        break;
                    }
                    case Flags.ZoneBound:
                    {
                        for (int y = 0; y < 64; y++)
                            for (int x = 0; x < 64; x++)
                                worldZoneBounds[x, y] = reader.ReadByte();
                        break;
                    }

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// Return world zone id at supplied position.
        /// </summary>
        public uint? GetWorldZoneId(Vector3 vector)
        {
            if ((flags & Flags.Zone) != 0)
            {
                if ((flags & Flags.ZoneBound) != 0)
                {
                    ZoneBoundFlags zoneFlags = GetZoneBoundFlags(vector);
                    if ((zoneFlags & ZoneBoundFlags.WorldZone2) != 0)
                        return worldZoneIds[2];
                    if ((zoneFlags & ZoneBoundFlags.WorldZone1) != 0)
                        return worldZoneIds[1];
                    if ((zoneFlags & ZoneBoundFlags.WorldZone0) != 0)
                        return worldZoneIds[0];
                }

                return worldZoneIds[0];
            }

            return null;
        }

        /// <summary>
        /// Return if Road March aura is active at supplied position.
        /// </summary>
        public bool HasRoadMarch(Vector3 vector)
        {
            return (GetZoneBoundFlags(vector) & ZoneBoundFlags.RoadMarch) != 0;
        }

        private ZoneBoundFlags GetZoneBoundFlags(Vector3 vector)
        {
            float x = vector.X + MapDefines.WorldGridOrigin * MapDefines.GridSize;
            float z = vector.Z + MapDefines.WorldGridOrigin * MapDefines.GridSize;
            uint localX = ((uint)Math.Floor(x) & 31) * 2;
            uint localZ = ((uint)Math.Floor(z) & 31) * 2;
            return (ZoneBoundFlags)worldZoneBounds[localX, localZ];
        }

        /// <summary>
        /// Return terrain height at supplied position.
        /// </summary>
        public float GetTerrainHeight(Vector3 vector)
        {
            float trueX = vector.X + MapDefines.WorldGridOrigin * MapDefines.GridSize;
            float trueZ = vector.Z + MapDefines.WorldGridOrigin * MapDefines.GridSize;

            uint vertexX = (uint)Math.Floor(trueX / 2f);
            uint localVertexX = vertexX & 15;
            uint vertexY = (uint)Math.Floor(trueZ / 2f);
            uint localVertexY = vertexY & 15;

            float p1 = heightMap[localVertexX + 1, localVertexY];
            float p2 = heightMap[localVertexX, localVertexY + 1];

            float sqX = (trueX / 2) - vertexX;
            float sqZ = (trueZ / 2) - vertexY;

            float height;
            if ((sqX + sqZ) < 1)
            {
                float p0 = heightMap[localVertexX, localVertexY];
                height = p0;
                height += (p1 - p0) * sqX;
                height += (p2 - p0) * sqZ;
            }
            else
            {
                float p3 = heightMap[localVertexX + 1, localVertexY + 1];
                height = p3;
                height += (p1 - p3) * (1.0f - sqZ);
                height += (p2 - p3) * (1.0f - sqX);
            }

            return height;
        }
    }
}
