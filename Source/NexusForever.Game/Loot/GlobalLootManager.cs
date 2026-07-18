using NexusForever.Database;
using NexusForever.Database.World;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Loot;
using NexusForever.Game.Entity;
using NexusForever.Game.Static.Account;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Loot;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Shared;
using NexusForever.Shared.Game;
using NLog;

namespace NexusForever.Game.Loot
{
    public class GlobalLootManager : Singleton<GlobalLootManager>, IGlobalLootManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<uint, List<ILootGroup>> creatureLoot = new Dictionary<uint, List<ILootGroup>>();
        private readonly Dictionary<uint, List<ILootGroup>> itemLoot = new Dictionary<uint, List<ILootGroup>>();

        private readonly List<ILootInstance> lootInstances = new List<ILootInstance>();

        // TODO: Investigate which GameFormula this comes from.
        // Right now, it's just outside Vacuum Loot Range
        private const float LOOT_RANGE = 35f;

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

            foreach (ItemLootModel itemLootModel in DatabaseManager.Instance.GetDatabase<WorldDatabase>().GetAllItemLootTables())
                BuildLoot(itemLootModel.Id, LootEntityType.Item, itemLootModel.LootGroup);

            foreach (EntityLootModel entityLootModel in DatabaseManager.Instance.GetDatabase<WorldDatabase>().GetAllEntityLootTables())
                BuildLoot(entityLootModel.Id, LootEntityType.Creature, entityLootModel.LootGroup);

            log.Info($"Loaded LootGroups for {itemLoot.Count} Item(s) and {creatureLoot.Count} Creature(s) in {(DateTime.Now - loadStarted).TotalMilliseconds}ms");
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
                        creatureLoot.Add(entityId, new List<ILootGroup>());

                    creatureLoot[entityId].Add(new LootGroup(lootGroupModel));
                    break;
                case LootEntityType.Item:
                    if (!itemLoot.ContainsKey(entityId))
                        itemLoot.Add(entityId, new List<ILootGroup>());

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
        private ILootInstance GenerateLootInstance(uint entityId, uint entityGuid, IPlayer player, Dictionary<ulong, uint> looterIds, LooterType looterType, LootEntityType lootEntityType)
        {
            ILootInstance lootInstance = new LootInstance(entityGuid, looterIds, looterType, lootEntityType);

            switch (lootEntityType)
            {
                case LootEntityType.Creature:
                    if (!creatureLoot.TryGetValue(entityId, out List<ILootGroup> creatureLootGroups))
                        return null;

                    foreach (ILootGroup lootGroup in creatureLootGroups)
                        foreach ((ILootItem item, uint count) in lootGroup.GenerateLootDrops(player))
                            lootInstance.AddLootItem(item.StaticId, item.Type, count);

                    break;
                case LootEntityType.Item:
                    if (!itemLoot.TryGetValue(entityId, out List<ILootGroup> itemLootGroups))
                        return null;

                    foreach (ILootGroup lootGroup in itemLootGroups)
                        foreach ((ILootItem item, uint count) in lootGroup.GenerateLootDrops(player))
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
        public ILootInstance DropLoot(IPlayer looter, IWorldEntity lootedEntity)
        {
            Creature2Entry entry = GameTableManager.Instance.Creature2.GetEntry(lootedEntity.CreatureId);
            if (entry == null)
                throw new InvalidOperationException($"Creature2 Entry {lootedEntity.CreatureId} not found.");

            if (!creatureLoot.ContainsKey(entry.Id))
                return null;

            // Build Dictionary of IDs for the Player. Need CharacterID in case player logs out and back in before loot expires.
            Dictionary<ulong, uint> playerIds = new Dictionary<ulong, uint>();
            if (looter == null)
                throw new InvalidOperationException($"Player not found on the WorldSession {looter.Account.Id}");
            playerIds.Add(looter.CharacterId, looter.Guid);

            ILootInstance lootInstance = GenerateLootInstance(entry.Id, lootedEntity.Guid, looter, playerIds, LooterType.Player, LootEntityType.Creature);
            if (lootInstance.HasExpired)
                return null;

            lootInstances.Add(lootInstance);

            lootInstance.SendLootNotify(looter);
            return lootInstance;
        }

        /// <summary>
        /// Drops loot for the <see cref="WorldSession"/> after the <see cref="Item"/> has been consumed.
        /// </summary>
        /// <remarks>This should mainly be used when a Player opens a Loot bag in their inventory.</remarks>
        public void DropLoot(IPlayer looter, IItem lootedItem)
        {
            if (!itemLoot.ContainsKey(lootedItem.Info.Id))
                return;

            // Build Dictionary of IDs for the Player. Need CharacterID in case player logs out and back in before loot expires.
            Dictionary<ulong, uint> playerIds = new Dictionary<ulong, uint>();
            if (looter == null)
                throw new InvalidOperationException($"Player not found on the WorldSession {looter.Account.Id}");

            playerIds.Add(looter.CharacterId, looter.Guid);

            ILootInstance lootInstance = GenerateLootInstance(lootedItem.Info.Id, looter.Guid, looter, playerIds, LooterType.Player, LootEntityType.Item);
            lootInstances.Add(lootInstance);

            // This baby's gonna blow!
            lootInstance.Explosion = true;

            lootInstance.SendLootNotify(looter);
        }

        /// <summary>
        /// Drops loot for the Group after the <see cref="WorldEntity"/> has been killed or destroyed.
        /// </summary>
        /// <remarks>This should mainly be used when a creature is killed by a Group.</remarks>
        public void DropLoot(uint groupId, IWorldEntity lootedEntity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return a <see cref="LootInstanceForItem"/> based on a given ID.
        /// </summary>
        private ILootInstance GetLootInstanceForItem(int lootInstanceItemId)
        {
            foreach (ILootInstance lootInstance in lootInstances)
            {
                if (!lootInstance.HasLootInstanceId(lootInstanceItemId))
                    continue;

                foreach (ILootInstanceItem item in lootInstance)
                    if (item.Id == lootInstanceItemId)
                        return lootInstance;
            }

            return null;
        }

        /// <summary>
        /// Return a <see cref="LootInstanceItem"/> based on a given ID.
        /// </summary>
        private ILootInstanceItem GetLootInstanceItem(int lootInstanceItemId)
        {
            foreach (ILootInstance lootInstance in lootInstances)
            {
                if (!lootInstance.HasLootInstanceId(lootInstanceItemId))
                    continue;

                foreach (ILootInstanceItem item in lootInstance)
                    if (item.Id == lootInstanceItemId)
                        return item;
            }

            return null;
        }

        /// <summary>
        /// Give <see cref="LootInstanceItem"/> to the <see cref="WorldSession"/> by a given ID. Should only be called by client handler.
        /// </summary>
        public void GiveLoot(IPlayer looter, int lootInstanceItemId)
        {
            ILootInstance lootInstance = GetLootInstanceForItem(lootInstanceItemId);
            if (lootInstance == null)
                throw new ArgumentNullException(nameof(lootInstanceItemId));

            if (!lootInstance.HasLooter(looter.CharacterId))
                throw new InvalidOperationException($"Character {looter.Name} ({looter.CharacterId}) is not meant to be able to loot instance tied to entity {lootInstance.Guid}.");

            IWorldEntity owner = looter.Map.GetEntity<IWorldEntity>(lootInstance.Guid);
            if (owner == null || owner.Position.GetDistance(looter.Position) > LOOT_RANGE)
                throw new InvalidOperationException($"Entity is null or out of range to loot.");

            if (lootInstance.HasExpired)
                throw new InvalidOperationException($"Trying to loot a LootInstance that has already expired or been looted.");

            foreach (ILootInstanceItem lootInstanceItem in lootInstance)
            {
                if (lootInstanceItem.Id != lootInstanceItemId)
                    continue;

                lootInstanceItem.SetWinner(looter.CharacterId, looter.Guid);
                lootInstanceItem.DeliverItem(looter);
            }

            if (lootInstance.HasExpired)
                owner.RemoveLoot(lootInstance);
        }

        /// <summary>
        /// Give all <see cref="LootInstanceItem"/> assigned to the <see cref="WorldSession"/> in range.
        /// </summary>
        public void GiveAllLootInRange(IPlayer looter)
        {
            if (looter == null)
                throw new ArgumentException();

            foreach (ILootInstance lootInstance in lootInstances.Where(p => p.HasLooter(looter.CharacterId)).ToList())
            {
                if (lootInstance.HasExpired)
                    continue;

                foreach (ILootInstanceItem item in lootInstance)
                    GiveLoot(looter, item.Id);
            }
        }

        /// <summary>
        /// Generate and deliver a single <see cref="LootInstanceItem"/> to a <see cref="WorldSession"/>, with a given <see cref="Item2Entry"/> and count.
        /// </summary>
        public void GiveLoot(IPlayer looter, Item2Entry entry, uint count, uint lootUnitId)
        {
            ILootInstanceItem lootInstance = new LootInstanceItem(entry.Id, LootItemType.StaticItem, count);
            lootInstance.SetWinner(looter.CharacterId, looter.Guid);
            lootInstance.SetLootUnit(lootUnitId);
            lootInstance.DeliverItem(looter);
        }

        /// <summary>
        /// Generate and deliver a single <see cref="LootInstanceItem"/> to a <see cref="WorldSession"/>, with a given <see cref="VirtualItemEntry"/> and count.
        /// </summary>
        public void GiveLoot(IPlayer looter, VirtualItemEntry entry, uint count, uint lootUnitId)
        {
            ILootInstanceItem lootInstance = new LootInstanceItem(entry.Id, LootItemType.VirtualItem, count);
            lootInstance.SetWinner(looter.CharacterId, looter.Guid);
            lootInstance.SetLootUnit(lootUnitId);
            lootInstance.DeliverItem(looter);
        }

        /// <summary>
        /// Generate and deliver a single <see cref="LootInstanceItem"/> to a <see cref="WorldSession"/>, with a given <see cref="AccountCurrencyType"/> and count.
        /// </summary>
        public void GiveLoot(IPlayer looter, AccountCurrencyType accountCurrencyType, uint count, uint lootUnitId)
        {
            ILootInstanceItem lootInstance = new LootInstanceItem((uint)accountCurrencyType, LootItemType.AccountCurrency, count);
            lootInstance.SetWinner(looter.CharacterId, looter.Guid);
            lootInstance.SetLootUnit(lootUnitId);
            lootInstance.DeliverItem(looter);
        }

        /// <summary>
        /// Generate and deliver a single <see cref="LootInstanceItem"/> to a <see cref="WorldSession"/>, with a given <see cref="CurrencyType"/> and count.
        /// </summary>
        public void GiveLoot(IPlayer looter, CurrencyType currencyType, uint count, uint lootUnitId)
        {
            ILootInstanceItem lootInstance = new LootInstanceItem((uint)currencyType, LootItemType.Cash, count);
            lootInstance.SetWinner(looter.CharacterId, looter.Guid);
            lootInstance.SetLootUnit(lootUnitId);
            lootInstance.DeliverItem(looter);
        }
    }
}
