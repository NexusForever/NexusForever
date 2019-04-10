using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.Map
{
    public class ZoneMapManager : ISaveCharacter
    {
        private const float ZoneMapBoundry = 32768.0F;
        private const byte RateLimit = 10;

        private readonly Player player;
        private ZoneMapCoordinate zoneMapCoordinate = new ZoneMapCoordinate();
        private Dictionary<ushort /*ZoneMapId*/, ZoneMap> ZoneMaps = new Dictionary<ushort, ZoneMap>();
        public ushort CurrentZoneMap { get; private set; }
        private Vector3 LastPosition;

        public ZoneMapManager(Player owner, Character characterModel)
        {
            player = owner;

            foreach(var characterZoneMapHexGroup in characterModel.CharacterZoneMapHexGroup)
            {
                ZoneMaps.TryGetValue(characterZoneMapHexGroup.ZoneMap, out ZoneMap zoneMap);
                if (zoneMap == null)
                {
                    zoneMap = new ZoneMap(characterZoneMapHexGroup.ZoneMap, player);
                    ZoneMaps.Add(characterZoneMapHexGroup.ZoneMap, zoneMap);
                }

                zoneMap.AddHexGroup(characterZoneMapHexGroup.HexGroup, true);
            }
        }

        public void Save(CharacterContext context)
        {
            foreach (var zoneMap in ZoneMaps)
                zoneMap.Value.Save(context);
        }

        public void Update(Vector3 vector)
        {
            if (player.Zone == null || CurrentZoneMap == 0 || player.IsLoading)
                return;

            // rate limit for e.g. micro movement when idle swimming in a current
            if (LastPosition.X < vector.X+RateLimit && LastPosition.Y < vector.Y+RateLimit && LastPosition.Z < vector.Z+RateLimit)
                return;

            LastPosition = vector;

            ZoneMaps.TryGetValue(CurrentZoneMap, out ZoneMap zoneMap);

            if (zoneMap.IsComplete)
                return;

            var newZoneMapCoordinate = Points2ZoneMapCoordinate(vector);

            if (zoneMapCoordinate?.X != newZoneMapCoordinate.X || zoneMapCoordinate?.Y != newZoneMapCoordinate.Y)
                zoneMapCoordinate = newZoneMapCoordinate;
            else
                return;

            foreach (var mapZoneHexGroup in GameTableManager.MapZoneHexGroup.Entries.Where(m => m.MapZoneId == CurrentZoneMap))
            {
                if (zoneMap.ZoneMapHexGroups.ContainsKey((ushort)mapZoneHexGroup.Id))
                    continue;

                // +/-1 is for proximity
                var mapZoneHexGroupEntry = GameTableManager.MapZoneHexGroupEntry.Entries.
                    FirstOrDefault(m => m.MapZoneHexGroupId == mapZoneHexGroup.Id &&
                                  ((m.HexX >= zoneMapCoordinate.X-1u &&
                                    m.HexX <= zoneMapCoordinate.X+1u)&&
                                   (m.HexY >= zoneMapCoordinate.Y-1u &&
                                    m.HexY <= zoneMapCoordinate.Y+1u))
                );

                if (mapZoneHexGroupEntry != null)
                    zoneMap.AddHexGroup((ushort)mapZoneHexGroup.Id);
            }
        }

        public ZoneMapCoordinate Points2ZoneMapCoordinate(Vector3 position)
        {
            var zoneMapCoordinate = new ZoneMapCoordinate();
            float a = (((ZoneMapBoundry + position.Z) / 27.712812F) + 1.0F);
            float b = ((ZoneMapBoundry + position.X) / 32.0F) + 0.5F;
            float c = (a*0.5F)+b;
            float d = c - (a * 2);
            float e = (float)((d * 0.33333334d) + 0.0000099999997d);

            zoneMapCoordinate.X = (ushort)((e * 2.0F) + a);
            zoneMapCoordinate.Y = (ushort)(a * 0.5F);

            return zoneMapCoordinate;
        }

        public void CalculateCurrentZoneMap()
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
                var mapZoneWorldJoin = GameTableManager.MapZoneWorldJoin.Entries.FirstOrDefault(m => m.WorldId == player.Map.Entry.Id);
                zoneMap = GameTableManager.MapZone.GetEntry(mapZoneWorldJoin.MapZoneId);
            }
            
            if (zoneMap == null && CurrentZoneMap != 0)
            {
                OnZoneMapChange((ushort)0u);
            }

            if (zoneMap?.Id != CurrentZoneMap)
            {
                if (!ZoneMaps.ContainsKey((ushort)zoneMap.Id))
                    ZoneMaps.Add((ushort)zoneMap.Id, new ZoneMap(zoneMap, player));

                OnZoneMapChange((ushort)zoneMap.Id);
            }

            return;
        }

        public void OnZoneUpdate()
        {
            CalculateCurrentZoneMap();
        }
        
        public void OnZoneMapChange(ushort newZoneMap)
        {
            CurrentZoneMap = newZoneMap;
        }

        public void SendZoneMaps()
        {
            foreach (var zoneMap in ZoneMaps)
                zoneMap.Value.Send();
        }

        public void SendInitialPackets()
        {
             SendZoneMaps();
        }
    }
}
