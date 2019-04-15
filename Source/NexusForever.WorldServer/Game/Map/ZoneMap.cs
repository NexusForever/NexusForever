using System.Collections.Generic;
using System.Linq;
using NexusForever.Shared.Network;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Map
{
    public class ZoneMapCoordinate
    {
        public ushort X { get; set; }
        public ushort Y { get; set; }
    }

    public class ZoneMap : ISaveCharacter
    {
        private MapZoneEntry entry;
        private readonly Player player;

        private ushort width;
        private ushort height;
        private ushort size;
        private ushort maxHexGroups;

        private NetworkBitArray zoneMapBits;
        public bool IsComplete { get; private set; }
        public Dictionary<ushort /*ZoneMapHexGroupId*/, bool /*new*/> ZoneMapHexGroups { get; } = new Dictionary<ushort, bool>();

        public ZoneMap(MapZoneEntry mapZone, Player owner)
        {
            player = owner;
            Setup(mapZone);
        }

        public ZoneMap(ushort mapZone, Player owner)
        {
            player = owner;
            MapZoneEntry mapZoneEntry = GameTableManager.MapZone.GetEntry(mapZone);
            Setup(mapZoneEntry);
        }

        private void Setup(MapZoneEntry mapZone)
        {
            entry = mapZone;
            width  = (ushort)(mapZone.HexLimX - mapZone.HexMinX +1);
            height = (ushort)(mapZone.HexLimY - mapZone.HexMinY +1);
            ushort wh = (ushort)(width * height);
            size = (ushort)((wh % 8u > 0u ? 8u : 0u) + wh);
            zoneMapBits = new NetworkBitArray(size);

            maxHexGroups = (ushort)GameTableManager.MapZoneHexGroup.Entries.Count(m => m.MapZoneId == entry.Id);
        }

        public void Send()
        {
            player.Session.EnqueueMessageEncrypted(new ServerZoneMap{
                ZoneMapId = entry.Id,
                ZoneMapBits = zoneMapBits,
                Count = size
            });
        }

        public void AddHexGroup(ushort hexGroupId, bool init = false)
        {
            foreach (MapZoneHexGroupEntryEntry mapZoneHexGroupEntry in GameTableManager.MapZoneHexGroupEntry.Entries.Where(m => m.MapZoneHexGroupId == hexGroupId))
            {
                var bit = (ushort)
                    (((short)mapZoneHexGroupEntry.HexY-(short)entry.HexMinY)*(short)width +
                    ((short)mapZoneHexGroupEntry.HexX-(short)entry.HexMinX));

                zoneMapBits.SetBit(bit, true, true);
            }

            ZoneMapHexGroups.Add(hexGroupId, !init);

            IsComplete = maxHexGroups == ZoneMapHexGroups.Count;
            if (!init)
                Send();
        }

        public void Save(CharacterContext context)
        {
            foreach (var zoneMapHexGroup in ZoneMapHexGroups.Where(z => z.Value).ToList())
            {
                var model = new CharacterZoneMapHexGroup
                {
                    Id = player.CharacterId,
                    ZoneMap = (ushort)entry.Id,
                    HexGroup = zoneMapHexGroup.Key
                };

                context.Add(model);
                ZoneMapHexGroups[zoneMapHexGroup.Key] = false;
            }
        }
    }
}
