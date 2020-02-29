using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared.Game.Map;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.Map
{
    public class ZoneMapManager : ISaveCharacter
    {
        private const float ZoneMapBoundary = MapDefines.WorldGridOrigin * MapDefines.GridSize;
        private const byte RateLimit = 10;

        private ushort currentZoneMap;
        private ZoneMapCoordinate currentZoneMapCoordinate = new ZoneMapCoordinate();
        private Vector3 lastPosition;

        private readonly Dictionary<ushort /*ZoneMapId*/, ZoneMap> zoneMaps = new Dictionary<ushort, ZoneMap>();

        private readonly Player player;

        /// <summary>
        /// Create a new <see cref="ZoneMapManager"/> from existing <see cref="CharacterModel"/> database model.
        /// </summary>
        public ZoneMapManager(Player owner, CharacterModel characterModel)
        {
            player = owner;

            foreach (CharacterZonemapHexgroupModel hexGroupModel in characterModel.ZonemapHexgroup)
            {
                if (!zoneMaps.TryGetValue(hexGroupModel.ZoneMap, out ZoneMap zoneMap))
                {
                    MapZoneEntry entry = GameTableManager.Instance.MapZone.GetEntry(hexGroupModel.ZoneMap);
                    zoneMap = new ZoneMap(entry, player);
                    zoneMaps.Add(hexGroupModel.ZoneMap, zoneMap);
                }

                zoneMap.AddHexGroup(hexGroupModel.HexGroup, false);
            }
        }

        public void Save(CharacterContext context)
        {
            foreach (ZoneMap zoneMap in zoneMaps.Values)
                zoneMap.Save(context);
        }

        public void SendInitialPackets()
        {
            SendZoneMaps();
        }

        public void SendZoneMaps()
        {
            foreach (ZoneMap zoneMap in zoneMaps.Values)
                zoneMap.Send();
        }

        /// <summary>
        /// Invoked when <see cref="Player"/> moves to a new <see cref="Vector3"/>.
        /// </summary>
        public void OnRelocate(Vector3 vector)
        {
            if (player.Zone == null || currentZoneMap == 0 || player.IsLoading)
                return;

            // rate limit for e.g. micro movement when idle swimming in a current
            if (lastPosition.X < vector.X+RateLimit && lastPosition.Y < vector.Y+RateLimit && lastPosition.Z < vector.Z+RateLimit)
                return;

            lastPosition = vector;

            if (zoneMaps.TryGetValue(currentZoneMap, out ZoneMap zoneMap) && zoneMap.IsComplete)
                return;

            ZoneMapCoordinate newZoneMapCoordinate = Points2ZoneMapCoordinate(vector);
            if (currentZoneMapCoordinate?.X == newZoneMapCoordinate.X && currentZoneMapCoordinate?.Y == newZoneMapCoordinate.Y)
                return;

            currentZoneMapCoordinate = newZoneMapCoordinate;

            foreach (MapZoneHexGroupEntry mapZoneHexGroup in GameTableManager.Instance.MapZoneHexGroup.Entries.Where(m => m.MapZoneId == currentZoneMap))
            {
                if (zoneMap != null && zoneMap.HasHexGroup((ushort)mapZoneHexGroup.Id))
                    continue;

                // +/-1 is for proximity
                MapZoneHexGroupEntryEntry mapZoneHexGroupEntry = GameTableManager.Instance.MapZoneHexGroupEntry.Entries.
                    FirstOrDefault(m => m.MapZoneHexGroupId == mapZoneHexGroup.Id
                        && m.HexX >= currentZoneMapCoordinate.X - 1u
                        && m.HexX <= currentZoneMapCoordinate.X + 1u
                        && m.HexY >= currentZoneMapCoordinate.Y - 1u
                        && m.HexY <= currentZoneMapCoordinate.Y + 1u
                );
                if (mapZoneHexGroupEntry == null)
                    continue;

                zoneMap?.AddHexGroup((ushort)mapZoneHexGroup.Id);
            }
        }

        private static ZoneMapCoordinate Points2ZoneMapCoordinate(Vector3 position)
        {
            float a = (ZoneMapBoundary + position.Z) / 27.712812f + 1f;
            float b = (ZoneMapBoundary + position.X) / 32f + 0.5f;
            float c = a * 0.5f + b;
            float d = c - a * 2f;
            float e = (float)(d * 0.33333334d + 0.0000099999997d);

            return new ZoneMapCoordinate
            {
                X = (ushort)(e * 2f + a),
                Y = (ushort)(a * 0.5f)
            };
        }

        /// <summary>
        /// Invoked when <see cref="Player"/> moves to a new zone.
        /// </summary>
        public void OnZoneUpdate()
        {
            // maybe there is a more efficient lookup method @sub_1406FB130 - this works for all zones though
            WorldZoneEntry worldZoneEntry = player.Zone;
            MapZoneEntry zoneMap = null;

            do
            {
                if (worldZoneEntry == null)
                    break;

                zoneMap = GameTableManager.Instance.MapZone.Entries.FirstOrDefault(m => m.WorldZoneId == worldZoneEntry.Id);
                if (zoneMap != null)
                    break;

                worldZoneEntry = GameTableManager.Instance.WorldZone.GetEntry(worldZoneEntry.ParentZoneId);
            }
            while (worldZoneEntry != null);

            if (zoneMap == null)
            {
                MapZoneWorldJoinEntry mapZoneWorldJoin = GameTableManager.Instance.MapZoneWorldJoin.Entries.FirstOrDefault(m => m.WorldId == player.Map.Entry.Id);
                if (mapZoneWorldJoin != null)
                    zoneMap = GameTableManager.Instance.MapZone.GetEntry(mapZoneWorldJoin.MapZoneId);
            }

            if (zoneMap == null)
            {
                currentZoneMap = 0;
                return;
            }

            if (zoneMap.Id == currentZoneMap)
                return;

            if (!zoneMaps.ContainsKey((ushort) zoneMap.Id))
                zoneMaps.Add((ushort)zoneMap.Id, new ZoneMap(zoneMap, player));

            currentZoneMap = (ushort)zoneMap.Id;
        }
    }
}
