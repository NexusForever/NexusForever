using NexusForever.Database.World.Model;
using NexusForever.Shared;
using NexusForever.Shared.Database;
using NexusForever.Shared.Game;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Account.Static;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Loot.Static;
using NexusForever.WorldServer.Network;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace NexusForever.WorldServer.Game.Loot
{
    public class GlobalLootManager : Singleton<GlobalLootManager>, IUpdate
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<uint, List<LootGroup>> creatureLoot = new Dictionary<uint, List<LootGroup>>();
        private readonly Dictionary<uint, List<LootGroup>> itemLoot = new Dictionary<uint, List<LootGroup>>();

        private readonly List<LootInstance> lootInstances = new List<LootInstance>();

        /// <summary>
        /// This ID is assigned to <see cref="LootItem"/> to be sent to the client.
        /// </summary>
        /// <remarks>This ID may've been unique per client when generating "solo" loot.</remarks>
        public int NextLootId => lootId + 1 != 0u ? lootId++ : lootId += 2;
        private int lootId = int.MinValue;

        private UpdateTimer updateTimer = new UpdateTimer(1d);

        public GlobalLootManager()
        {
        }

        /// <summary>
        /// Initialise this <see cref="GlobalLootManager"/> Instance.
        /// </summary>
        public void Initialise()
        {
            DateTime loadStarted = DateTime.Now;

            foreach (ItemLootModel itemLootModel in DatabaseManager.Instance.WorldDatabase.GetAllItemLootTables())
                BuildLoot(itemLootModel.Id, LootEntityType.Item, itemLootModel.LootGroup);

            log.Info($"Loaded LootGroups for {itemLoot.Count} Item(s) and {0} Creature(s) in {(DateTime.Now - loadStarted).TotalMilliseconds}ms");
        }

        /// <summary>
        /// Builds all <see cref="LootGroup"/> for the given <see cref="LootGroupModel"/>, based on <see cref="LootEntityType"/> and ID.
        /// </summary>
        private void BuildLoot(uint entityId, LootEntityType type, LootGroupModel lootGroupModel)
        {
            switch (type)
            {
                case LootEntityType.Creature:
                    if (!creatureLoot.ContainsKey(entityId))
                        creatureLoot.Add(entityId, new List<LootGroup>());

                    creatureLoot[entityId].Add(new LootGroup(lootGroupModel));
                    break;
                case LootEntityType.Item:
                    if (!itemLoot.ContainsKey(entityId))
                        itemLoot.Add(entityId, new List<LootGroup>());

                    itemLoot[entityId].Add(new LootGroup(lootGroupModel));
                    break;
            }
        }

        /// <summary>
        /// Called on every server Update.
        /// </summary>
        public void Update(double lastTick)
        {
            updateTimer.Update(lastTick);

            foreach (LootInstance lootInstance in lootInstances)
                lootInstance.Update(lastTick);

            if (updateTimer.HasElapsed)
            {
                RemoveExpiredLootInstances();

                updateTimer.Reset();
            }
        }

        /// <summary>
        /// Removes all <see cref="LootInstance"/> that have now expired (either claimed or timed out).
        /// </summary>
        private void RemoveExpiredLootInstances()
        {
            foreach (LootInstance lootInstance in lootInstances.Where(i => i.HasExpired).ToList())
                lootInstances.Remove(lootInstance);
        }

        /// <summary>
        /// Generate a new <see cref="LootInstance"/> based on the <see cref="LootEntityType"/> and ID from the cached <see cref="LootGroup"/>.
        /// </summary>
        private LootInstance GenerateLootInstance(uint entityId, uint entityGuid, Dictionary<ulong, uint> looterIds, LooterType looterType, LootEntityType lootEntityType)
        {
            LootInstance lootInstance = new LootInstance(entityGuid, looterIds, looterType, lootEntityType);

            switch (lootEntityType)
            {
                case LootEntityType.Creature:
                    if (!creatureLoot.TryGetValue(entityId, out List<LootGroup> creatureLootGroups))
                        return null;

                    foreach (LootGroup lootGroup in creatureLootGroups)
                        foreach ((LootItem item, uint count) in lootGroup.GenerateLootDrops())
                            lootInstance.AddLootItem(item.StaticId, item.Type, count);

                    break;
                case LootEntityType.Item:
                    if (!itemLoot.TryGetValue(entityId, out List<LootGroup> itemLootGroups))
                        return null;

                    foreach (LootGroup lootGroup in itemLootGroups)
                        foreach ((LootItem item, uint count) in lootGroup.GenerateLootDrops())
                            lootInstance.AddLootItem(item.StaticId, item.Type, count);

                    break;
            }

            // TODO: Generate currency rewards extra?

            // TODO: Generate account currency rewards extra?

            return lootInstance;
        }

        /// <summary>
        /// Drops loot for the <see cref="WorldSession"/> after the <see cref="WorldEntity"/> has been killed or destroyed.
        /// </summary>
        /// <remarks>This should mainly be used when a creature is killed by the Player.</remarks>
        public void DropLoot(WorldSession looter, WorldEntity lootedEntity)
        {
            Creature2Entry entry = GameTableManager.Instance.Creature2.GetEntry(lootedEntity.CreatureId);
            if (entry == null)
                throw new InvalidOperationException($"Creature2 Entry {lootedEntity.CreatureId} not found.");

            if (!creatureLoot.ContainsKey(entry.Id))
                return;

            // Build Dictionary of IDs for the Player. Need CharacterID in case player logs out and back in before loot expires.
            Dictionary<ulong, uint> playerIds = new Dictionary<ulong, uint>();
            if (looter.Player == null)
                throw new InvalidOperationException($"Player not found on the WorldSession {looter.Account.Id}");
            playerIds.Add(looter.Player.CharacterId, looter.Player.Guid);

            LootInstance lootInstance = GenerateLootInstance(entry.Id, lootedEntity.Guid, playerIds, LooterType.Player, LootEntityType.Creature);
            lootInstances.Add(lootInstance);

            lootInstance.SendLootNotify(looter);
        }

        /// <summary>
        /// Drops loot for the <see cref="WorldSession"/> after the <see cref="Item"/> has been consumed.
        /// </summary>
        /// <remarks>This should mainly be used when a Player opens a Loot bag in their inventory.</remarks>
        public void DropLoot(WorldSession looter, Item lootedItem)
        {
            if (!itemLoot.ContainsKey(lootedItem.Entry.Id))
                return;

            // Build Dictionary of IDs for the Player. Need CharacterID in case player logs out and back in before loot expires.
            Dictionary<ulong, uint> playerIds = new Dictionary<ulong, uint>();
            if (looter.Player == null)
                throw new InvalidOperationException($"Player not found on the WorldSession {looter.Account.Id}");
            playerIds.Add(looter.Player.CharacterId, looter.Player.Guid);

            LootInstance lootInstance = GenerateLootInstance(lootedItem.Entry.Id, looter.Player.Guid, playerIds, LooterType.Player, LootEntityType.Item);
            lootInstances.Add(lootInstance);

            // This baby's gonna blow!
            lootInstance.Explosion = true;

            lootInstance.SendLootNotify(looter);
        }

        /// <summary>
        /// Drops loot for the Group after the <see cref="WorldEntity"/> has been killed or destroyed.
        /// </summary>
        /// <remarks>This should mainly be used when a creature is killed by a Group.</remarks>
        public void DropLoot(uint groupId, WorldEntity lootedEntity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return a <see cref="LootInstanceItem"/> based on a given ID.
        /// </summary>
        private LootInstanceItem GetLootInstanceItem(int lootInstanceItemId)
        {
            foreach (LootInstance lootInstance in lootInstances)
            {
                if (!lootInstance.HasLootInstanceId(lootInstanceItemId))
                    continue;

                foreach (LootInstanceItem item in lootInstance)
                    if (item.Id == lootInstanceItemId)
                        return item;
            }

            return null;
        }

        /// <summary>
        /// Give <see cref="LootInstanceItem"/> to the <see cref="WorldSession"/> by a given ID. Should only be called by client handler.
        /// </summary>
        public void GiveLoot(WorldSession looter, int lootInstanceItemId)
        {
            LootInstanceItem lootInstanceItem = GetLootInstanceItem(lootInstanceItemId);
            if (lootInstanceItem == null)
                throw new ArgumentNullException(nameof(lootInstanceItemId));

            lootInstanceItem.SetWinner(looter.Player.CharacterId, looter.Player.Guid);
            lootInstanceItem.DeliverItem(looter);
        }

        /// <summary>
        /// Generate and deliver a single <see cref="LootInstanceItem"/> to a <see cref="WorldSession"/>, with a given <see cref="Item2Entry"/> and count.
        /// </summary>
        public void GiveLoot(WorldSession looter, Item2Entry entry, uint count)
        {
            LootInstanceItem lootInstance = new LootInstanceItem(entry.Id, LootItemType.StaticItem, count);
            lootInstance.DeliverItem(looter);
        }

        /// <summary>
        /// Generate and deliver a single <see cref="LootInstanceItem"/> to a <see cref="WorldSession"/>, with a given <see cref="VirtualItemEntry"/> and count.
        /// </summary>
        public void GiveLoot(WorldSession looter, VirtualItemEntry entry, uint count)
        {
            LootInstanceItem lootInstance = new LootInstanceItem(entry.Id, LootItemType.VirtualItem, count);
            lootInstance.DeliverItem(looter);
        }

        /// <summary>
        /// Generate and deliver a single <see cref="LootInstanceItem"/> to a <see cref="WorldSession"/>, with a given <see cref="AccountCurrencyType"/> and count.
        /// </summary>
        public void GiveLoot(WorldSession looter, AccountCurrencyType accountCurrencyType, uint count)
        {
            LootInstanceItem lootInstance = new LootInstanceItem((uint)accountCurrencyType, LootItemType.AccountCurrency, count);
            lootInstance.DeliverItem(looter);
        }

        /// <summary>
        /// Generate and deliver a single <see cref="LootInstanceItem"/> to a <see cref="WorldSession"/>, with a given <see cref="CurrencyType"/> and count.
        /// </summary>
        public void GiveLoot(WorldSession looter, CurrencyType currencyType, uint count)
        {
            LootInstanceItem lootInstance = new LootInstanceItem((uint)currencyType, LootItemType.Cash, count);
            lootInstance.DeliverItem(looter);
        }
    }
}
