using System.Numerics;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Entity;
using NexusForever.Game.Static.Achievement;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.IO.Map;

namespace NexusForever.Game.Map
{
    public class ZoneMapManager : IZoneMapManager
    {
        private const float ZoneMapBoundary = MapDefines.WorldGridOrigin * MapDefines.GridSize;
        private const byte RateLimit = 10;

        private ushort currentZoneMap;
        private ZoneMapCoordinate currentZoneMapCoordinate = new();
        private Vector3 lastPosition;

        private readonly Dictionary<ushort /*ZoneMapId*/, IZoneMap> zoneMaps = new();

        private readonly Player player;

        /// <summary>
        /// Create a new <see cref="IZoneMapManager"/> from existing <see cref="CharacterModel"/> database model.
        /// </summary>
        public ZoneMapManager(Player owner, CharacterModel characterModel)
        {
            player = owner;

            foreach (CharacterZonemapHexgroupModel hexGroupModel in characterModel.ZonemapHexgroup)
            {
                if (!zoneMaps.TryGetValue(hexGroupModel.ZoneMap, out IZoneMap zoneMap))
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
            foreach (IZoneMap zoneMap in zoneMaps.Values)
                zoneMap.Save(context);
        }

        public void SendInitialPackets()
        {
            SendZoneMaps();
        }

        public void SendZoneMaps()
        {
            foreach (IZoneMap zoneMap in zoneMaps.Values)
                zoneMap.Send();
        }

        /// <summary>
        /// Invoked when <see cref="IPlayer"/> moves to a new <see cref="Vector3"/>.
        /// </summary>
        public void OnRelocate(Vector3 vector)
        {
            if (player.Zone == null || currentZoneMap == 0 || player.IsLoading)
                return;

            // rate limit for e.g. micro movement when idle swimming in a current
            if (Vector3.Distance(lastPosition, vector) < RateLimit)
                return;

            lastPosition = vector;

            if (zoneMaps.TryGetValue(currentZoneMap, out IZoneMap zoneMap) && zoneMap.IsComplete)
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

            if (zoneMap.IsComplete)
                player.AchievementManager.CheckAchievements(player, AchievementType.MapComplete, currentZoneMap);
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
        /// Invoked when <see cref="IPlayer"/> moves to a new zone.
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
