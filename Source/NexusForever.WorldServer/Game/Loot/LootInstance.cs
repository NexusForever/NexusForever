using NexusForever.Shared.Game;
using NexusForever.Shared;
using NexusForever.WorldServer.Game.Loot.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using NetworkLootItem = NexusForever.WorldServer.Network.Message.Model.Shared.LootItem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NexusForever.WorldServer.Game.Loot
{
    public class LootInstance : IEnumerable<LootInstanceItem>, IUpdate
    {
        public uint Guid { get; }
        public LootEntityType LootEntityType { get; }
        public LooterType LooterType { get; }
        public bool Explosion { get; set; }

        public bool HasExpired => expiryTimer.HasElapsed || lootItems.Values.FirstOrDefault(i => i.Delivered == false) == null;

        private Dictionary</* characterId */ ulong, /* guid */ uint> looterGuids { get; } = new Dictionary<ulong, uint>();
        private Dictionary</*uniqueId*/ int, LootInstanceItem> lootItems = new Dictionary<int, LootInstanceItem>();

        private UpdateTimer expiryTimer = new UpdateTimer(1800d);

        /// <summary>
        /// Create a new <see cref="LootInstance"/>.
        /// </summary>
        public LootInstance(uint unitGuid, Dictionary<ulong, uint> looterIds, LooterType looterType, LootEntityType lootEntityType)
        {
            Guid = unitGuid;
            LootEntityType = lootEntityType;
            LooterType = looterType;

            foreach ((ulong characterId, uint guid) in looterIds)
                looterGuids.Add(characterId, guid);            
        }

        /// <summary>
        /// Updates this <see cref="LootInstance"/> expiry timer.
        /// </summary>
        public void Update(double lastTick)
        {
            if (expiryTimer.HasElapsed)
                return;

            expiryTimer.Update(lastTick);
        }

        /// <summary>
        /// Create a new <see cref="LootInstanceItem"/> for this <see cref="LootInstance"/>.
        /// </summary>
        public void AddLootItem(uint staticId, LootItemType type, uint count)
        {
            LootInstanceItem item = new LootInstanceItem(staticId, type, count);
            lootItems.Add(item.Id, item);

            if (LootEntityType == LootEntityType.Item)
                item.SetWinner(looterGuids.First().Key, looterGuids.First().Value);
        }

        /// <summary>
        /// Delivers all <see cref="LootInstanceItem"/> rewards to the given winner. Should only be used when a Player opens a Loot Bag.
        /// </summary>
        private void DeliverAllItemsImmediately(WorldSession session)
        {
            foreach (LootInstanceItem item in lootItems.Values)
                item.DeliverItem(session, false);
        }

        /// <summary>
        /// Delivers any already won Items to the <see cref="WorldSession"/> and sends a <see cref="ServerLootNotify"/> packet.
        /// </summary>
        public void SendLootNotify(WorldSession session)
        {
            if (LootEntityType == LootEntityType.Item && LooterType == LooterType.Player)
                DeliverAllItemsImmediately(session);

            List<NetworkLootItem> networkLootItems = new List<NetworkLootItem>();

            foreach (LootInstanceItem item in lootItems.Values)
            {
                if (item.Type == LootItemType.AccountCurrency)
                {
                    networkLootItems.AddRange(item.BuildForAccountCurrency());
                    continue;
                }
                
                NetworkLootItem networkLootItem = item.Build();
                networkLootItem.CanLoot = looterGuids.Keys.Contains(session.Player.CharacterId);
                networkLootItem.Granted = item.Delivered;
                if (item.Delivered)
                    networkLootItem.UniqueId = 0;

                networkLootItems.Add(networkLootItem);
            }

            session.EnqueueMessageEncrypted(new ServerLootNotify
            {
                UnitId = Guid,
                Explosion = Explosion,
                LootItems = networkLootItems
            });
        }

        /// <summary>
        /// Returns true if this <see cref="LootInstance"/> has a <see cref="LootInstanceItem"/> with the given ID.
        /// </summary>
        public bool HasLootInstanceId(int lootInstanceId)
        {
            return lootItems.Keys.Contains(lootInstanceId);
        }

        public IEnumerator<LootInstanceItem> GetEnumerator()
        {
            return lootItems.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
