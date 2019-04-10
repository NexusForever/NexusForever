using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Shared.Network;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Game.Map
{
    public class ZoneMapCoordinate
    {
        public ushort X { get; set; }
        public ushort Y { get; set; }
    }

    public class ZoneMap : ISaveCharacter
    {
        public MapZoneEntry Entry { get; private set; }
        private readonly Player player;

        private ushort Width;
        private ushort Heigth;
        private ushort Size;
        private ushort MaxHexGroups;

        private NetworkBitArray ZoneMapBits;
        public bool IsComplete { get; private set; } = false;
        public Dictionary<ushort /*ZoneMapHexGroupId*/, bool /*new*/> ZoneMapHexGroups { get; private set; } = new Dictionary<ushort, bool>();

        public ZoneMap(MapZoneEntry mapZone, Player owner)
        {
            player = owner;
            Setup(mapZone, owner);
        }

        public ZoneMap(ushort mapZone, Player owner)
        {
            player = owner;
            MapZoneEntry mapZoneEntry = GameTableManager.MapZone.GetEntry(mapZone);
            Setup(mapZoneEntry, owner);
        }

        public void Setup(MapZoneEntry mapZone, Player owner)
        {
            Entry = mapZone;
            Width  = (ushort)(mapZone.HexLimX - mapZone.HexMinX +1);
            Heigth = (ushort)(mapZone.HexLimY - mapZone.HexMinY +1);
            ushort wh = (ushort)(Width * Heigth);
            Size = (ushort)((wh % 8u > 0u ? 8u : 0u) + wh);
            ZoneMapBits = new NetworkBitArray(Size);

            MaxHexGroups = (ushort)GameTableManager.MapZoneHexGroup.Entries.Count(m => m.MapZoneId == Entry.Id);
        }

        public void Send()
        {
            player.Session.EnqueueMessageEncrypted(new ServerZoneMap{
                ZoneMapId = Entry.Id,
                ZoneMapBits = ZoneMapBits,
                Count = Size
            });
        }

        public void AddHexGroup(ushort hexGroupId, bool init = false)
        {
            foreach (var mapZoneHexGroupEntry in GameTableManager.MapZoneHexGroupEntry.Entries.Where(m => m.MapZoneHexGroupId == hexGroupId))
            {
                ushort bit = (ushort)
                    ((((short)mapZoneHexGroupEntry.HexY-(short)Entry.HexMinY)*(short)Width) +
                    ((short)mapZoneHexGroupEntry.HexX-(short)Entry.HexMinX));

                ZoneMapBits.SetBit(bit, true, true);
            }

            ZoneMapHexGroups.Add(hexGroupId, !init);
            IsComplete = MaxHexGroups == ZoneMapHexGroups.Count;
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
                    ZoneMap = (ushort)Entry.Id,
                    HexGroup = zoneMapHexGroup.Key
                };

                context.Add(model);
                ZoneMapHexGroups[zoneMapHexGroup.Key] = false;
            }
        }
    }
}
