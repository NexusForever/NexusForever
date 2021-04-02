using System.Collections.Generic;
using System.Linq;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared.Network;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Network.Message.Model;
using System;

namespace NexusForever.WorldServer.Game.Map
{
    public class ZoneMap : ISaveCharacter
    {
        /// <summary>
        /// Returns if all <see cref="MapZoneHexGroupEntry"/> in the <see cref="MapZoneEntry"/> have been discovered.
        /// </summary>
        public bool IsComplete => maxHexGroups == zoneMapHexGroups.Count;

        private readonly MapZoneEntry entry;
        
        private readonly ushort width;
        private readonly ushort height;
        private readonly ushort size;
        private readonly ushort maxHexGroups;
        private readonly uint maxHexCount;

        private readonly NetworkBitArray zoneMapBits;
        private readonly Dictionary<ushort /*ZoneMapHexGroupId*/, bool /*new*/> zoneMapHexGroups = new();

        private readonly Player player;

        /// <summary>
        /// Create a new <see cref="ZoneMapManager"/> from supplied <see cref="MapZoneEntry"/>.
        /// </summary>
        public ZoneMap(MapZoneEntry mapZone, Player owner)
        {
            player       = owner;

            entry        = mapZone;
            width        = (ushort)(mapZone.HexLimX - mapZone.HexMinX + 1);
            height       = (ushort)(mapZone.HexLimY - mapZone.HexMinY + 1);
            ushort wh    = (ushort)(width * height);
            size         = (ushort)((wh % 8u > 0u ? 8u : 0u) + wh);
            maxHexGroups = (ushort)GameTableManager.Instance.MapZoneHexGroup.Entries.Count(m => m.MapZoneId == entry.Id);
            zoneMapBits  = new NetworkBitArray(size, NetworkBitArray.BitOrder.LeastSignificantBit);

            uint hexCount = 0;
            foreach (MapZoneHexGroupEntry groupEntry in GameTableManager.Instance.MapZoneHexGroup.Entries.Where(m => m.MapZoneId == entry.Id))
                hexCount += (uint)GameTableManager.Instance.MapZoneHexGroupEntry.Entries.Count(m => m.MapZoneHexGroupId == groupEntry.Id);

            maxHexCount = hexCount;
        }

        public void Save(CharacterContext context)
        {
            foreach ((ushort hexGroupId, bool _) in zoneMapHexGroups.Where(z => z.Value).ToList())
            {
                var model = new CharacterZonemapHexgroupModel
                {
                    Id       = player.CharacterId,
                    ZoneMap  = (ushort)entry.Id,
                    HexGroup = hexGroupId
                };

                context.Add(model);
                zoneMapHexGroups[hexGroupId] = false;
            }
        }

        /// <summary>
        /// Send <see cref="ServerZoneMap"/> to <see cref="Player"/>.
        /// </summary>
        public void Send()
        {
            player.Session.EnqueueMessageEncrypted(new ServerZoneMap
            {
                ZoneMapId   = entry.Id,
                ZoneMapBits = zoneMapBits,
                Count       = size
            });
        }

        /// <summary>
        /// Add a new discovered <see cref="MapZoneHexGroupEntry"/>.
        /// </summary>
        public void AddHexGroup(ushort hexGroupId, bool sendUpdate = true)
        {
            foreach (MapZoneHexGroupEntryEntry mapZoneHexGroupEntry in GameTableManager.Instance.MapZoneHexGroupEntry.Entries.Where(m => m.MapZoneHexGroupId == hexGroupId))
            {
                var bit = (ushort)
                    (((short)mapZoneHexGroupEntry.HexY - (short)entry.HexMinY) * (short)width +
                    ((short)mapZoneHexGroupEntry.HexX - (short)entry.HexMinX));

                zoneMapBits.SetBit(bit, true);
            }

            zoneMapHexGroups.Add(hexGroupId, sendUpdate);
            if (sendUpdate)
                Send();
        }

        /// <summary>
        /// Returns if the supplied <see cref="MapZoneHexGroupEntry"/> has been discovered.
        /// </summary>
        public bool HasHexGroup(ushort hexGroupId)
        {
            return zoneMapHexGroups.ContainsKey(hexGroupId);
        }

        /// <summary>
        /// Returns the explored percentage of this <see cref="ZoneMap"/> explored.
        /// </summary>
        /// <remarks>This is here for testing purposes only. Confirmed that client % matched this % through the course of exploring multiple maps.</remarks>
        public float GetExploredPercent()
        {
            return (float)Math.Floor(((float)GetCurrentHexCount() / (float)maxHexCount) * 100f);
        }

        private int GetCurrentHexCount()
        {
            int count = 0;

            foreach (MapZoneHexGroupEntry groupEntry in GameTableManager.Instance.MapZoneHexGroup.Entries.Where(m => m.MapZoneId == entry.Id))
            {
                if (!zoneMapHexGroups.ContainsKey((ushort)groupEntry.Id))
                    continue;

                count += GameTableManager.Instance.MapZoneHexGroupEntry.Entries.Count(m => m.MapZoneHexGroupId == groupEntry.Id);
            }

            return count;
        }
    }
}
