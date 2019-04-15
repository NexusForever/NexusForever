using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.Map
{
    public class ZoneMapManager : ISaveCharacter
    {
        private const float ZoneMapBoundary = 32768.0F;
        private const byte RateLimit = 10;

        private readonly Player player;
        private ZoneMapCoordinate zoneMapCoordinate = new ZoneMapCoordinate();
        private readonly Dictionary<ushort /*ZoneMapId*/, ZoneMap> zoneMaps = new Dictionary<ushort, ZoneMap>();
        private ushort currentZoneMap;
        private Vector3 lastPosition;

        public ZoneMapManager(Player owner, Character characterModel)
        {
            player = owner;

            foreach(CharacterZoneMapHexGroup characterZoneMapHexGroup in characterModel.CharacterZoneMapHexGroup)
            {
                if(!zoneMaps.TryGetValue(characterZoneMapHexGroup.ZoneMap, out ZoneMap zoneMap))
                {
                    zoneMap = new ZoneMap(characterZoneMapHexGroup.ZoneMap, player);
                    zoneMaps.Add(characterZoneMapHexGroup.ZoneMap, zoneMap);
                }

                zoneMap.AddHexGroup(characterZoneMapHexGroup.HexGroup, true);
            }
        }

        public void Save(CharacterContext context)
        {
            foreach (var zoneMap in zoneMaps)
                zoneMap.Value.Save(context);
        }

        public void Update(Vector3 vector)
        {
            if (player.Zone == null || currentZoneMap == 0 || player.IsLoading)
                return;

            // rate limit for e.g. micro movement when idle swimming in a current
            if (lastPosition.X < vector.X+RateLimit && lastPosition.Y < vector.Y+RateLimit && lastPosition.Z < vector.Z+RateLimit)
                return;

            lastPosition = vector;

            if(zoneMaps.TryGetValue(currentZoneMap, out ZoneMap zoneMap))
                if (zoneMap.IsComplete)
                    return;

            ZoneMapCoordinate newZoneMapCoordinate = Points2ZoneMapCoordinate(vector);

            if (zoneMapCoordinate?.X != newZoneMapCoordinate.X || zoneMapCoordinate?.Y != newZoneMapCoordinate.Y)
                zoneMapCoordinate = newZoneMapCoordinate;
            else
                return;

            foreach (MapZoneHexGroupEntry mapZoneHexGroup in GameTableManager.MapZoneHexGroup.Entries.Where(m => m.MapZoneId == currentZoneMap))
            {
                if (zoneMap != null && zoneMap.ZoneMapHexGroups.ContainsKey((ushort)mapZoneHexGroup.Id))
                    continue;

                // +/-1 is for proximity
                MapZoneHexGroupEntryEntry mapZoneHexGroupEntry = GameTableManager.MapZoneHexGroupEntry.Entries.
                    FirstOrDefault(m => m.MapZoneHexGroupId == mapZoneHexGroup.Id &&
                                        m.HexX >= zoneMapCoordinate.X-1u &&
                                        m.HexX <= zoneMapCoordinate.X+1u &&
                                        m.HexY >= zoneMapCoordinate.Y-1u &&
                                        m.HexY <= zoneMapCoordinate.Y+1u
                );

                if (mapZoneHexGroupEntry == null)
                    continue;

                zoneMap?.AddHexGroup((ushort) mapZoneHexGroup.Id);
            }
        }

        private static ZoneMapCoordinate Points2ZoneMapCoordinate(Vector3 position)
        {
            var coordinate = new ZoneMapCoordinate();
            float a = (ZoneMapBoundary + position.Z) / 27.712812F + 1.0F;
            float b = (ZoneMapBoundary + position.X) / 32.0F + 0.5F;
            float c = a*0.5F+b;
            float d = c - a * 2;
            float e = (float)(d * 0.33333334d + 0.0000099999997d);

            coordinate.X = (ushort)(e * 2.0F + a);
            coordinate.Y = (ushort)(a * 0.5F);

            return coordinate;
        }

        private void CalculateCurrentZoneMap()
        {
            // maybe there is a more efficient lookup method @sub_1406FB130 - this works for all zones though
            WorldZoneEntry worldZoneEntry = player.Zone;
            MapZoneEntry zoneMap;

            do
            {
                zoneMap = GameTableManager.MapZone.Entries.FirstOrDefault(m => m.WorldZoneId == worldZoneEntry.Id);

                if (zoneMap != null || worldZoneEntry == null)
                    break;

                worldZoneEntry = GameTableManager.WorldZone.GetEntry(worldZoneEntry.ParentZoneId);
            }
            while(worldZoneEntry != null);


            if (zoneMap == null)
            {
                MapZoneWorldJoinEntry mapZoneWorldJoin = GameTableManager.MapZoneWorldJoin.Entries.FirstOrDefault(m => m.WorldId == player.Map.Entry.Id);
                if (mapZoneWorldJoin != null)
                    zoneMap = GameTableManager.MapZone.GetEntry(mapZoneWorldJoin.MapZoneId);
            }

            if (zoneMap == null)
            {
                if (currentZoneMap != 0)
                    OnZoneMapChange((ushort)0u);
                return;
            }

            if (zoneMap.Id == currentZoneMap)
                return;

            if (!zoneMaps.ContainsKey((ushort) zoneMap.Id))
                zoneMaps.Add((ushort) zoneMap.Id, new ZoneMap(zoneMap, player));

            OnZoneMapChange((ushort) zoneMap.Id);
        }

        public void OnZoneUpdate()
        {
            CalculateCurrentZoneMap();
        }

        public void OnZoneMapChange(ushort newZoneMap)
        {
            currentZoneMap = newZoneMap;
        }

        public void SendZoneMaps()
        {
            foreach (var zoneMap in zoneMaps)
                zoneMap.Value.Send();
        }

        public void SendInitialPackets()
        {
             SendZoneMaps();
        }
    }
}
