using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Abstract.Loot;
using NexusForever.Game.Abstract.Loot.Generate;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Static.Group;
using NexusForever.Game.Static.Loot;
using NexusForever.Shared;

namespace NexusForever.Game.Loot
{
    public class LootManager : ILootManager
    {
        public IBaseMap Map { get; private set; }

        private readonly Dictionary<uint, List<ILootInstance>> lootInstances = [];

        #region Dependency Injection

        private readonly IItemManager itemManager;
        private readonly ILootGroupManager lootGroupManager;
        private readonly IFactory<ILootInstance> lootInstanceFactory;

        public LootManager(
            IItemManager itemManager,
            ILootGroupManager lootGroupManager,
            IFactory<ILootInstance> lootInstanceFactory)
        {
            this.itemManager         = itemManager;
            this.lootGroupManager    = lootGroupManager;
            this.lootInstanceFactory = lootInstanceFactory;
        }

        #endregion

        public void Initialise(IBaseMap map)
        {
            Map = map;
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            foreach (List<ILootInstance> entityLootInstances in lootInstances.Values)
            {
                // reverse list iteration so we can remove expired elements
                for (int i = entityLootInstances.Count - 1; i >= 0; i--)
                {
                    ILootInstance lootInstance = entityLootInstances[i];
                    lootInstance.Update(lastTick);
                    if (lootInstance.Expired)
                    {
                        lootInstance.SendLootRemove();
                        entityLootInstances.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// Gets all loot instances associated with the specified unit.
        /// </summary>
        /// <param name="ownerUnitId">The id of the unit that owns the loot instances.</param>
        /// <returns>
        /// The loot instances associated with the specified unit, or an empty collection if none exist.
        /// </returns>
        public IEnumerable<ILootInstance> GetLootInstances(uint ownerUnitId)
        {
            return lootInstances.TryGetValue(ownerUnitId, out List<ILootInstance> entityLootInstances) ? entityLootInstances : [];
        }

        /// <summary>
        /// Get the loot item instance for the specified owner unit and loot id.
        /// </summary>
        /// <param name="ownerUnitId">The id of the unit that owns the loot instances.</param>
        /// <param name="lootId">The id of the loot item to retrieve.</param>
        /// <returns>
        /// The loot item instance, or null if not found.
        /// </returns>
        public ILootInstanceItem GetLootItemInstance(uint ownerUnitId, uint lootId)
        {
            foreach (ILootInstance lootInstance in GetLootInstances(ownerUnitId))
            {
                ILootInstanceItem lootInstanceItem = lootInstance.GetLootItem(lootId);
                if (lootInstanceItem != null)
                    return lootInstanceItem;
            }

            return null;
        }

        /// <summary>
        /// Generates loot for the specified player from a looted item.
        /// </summary>
        /// <param name="looter">The player who looted the item.</param>
        /// <param name="lootedItem">The item that was looted.</param>
        public void GenerateLoot(IPlayer looter, IItem lootedItem)
        {
            ILootBuilder lootBuilder = lootGroupManager.GenerateItemLoot(lootedItem.Id, [ looter ]);
            if (lootBuilder == null)
                return;

            ILootInstance lootInstance = lootInstanceFactory.Resolve();
            lootInstance.Initialise(this, looter);

            foreach (ILootBuilderItem lootBuilderItem in lootBuilder.GetItems())
            {
                ILootInstanceItem lootInstanceItem = lootInstance.AddLootItem(lootBuilderItem);
                lootInstanceItem.SetWinner(looter.Identity);
                lootInstanceItem.DeliverLoot(looter);
            }

            lootInstance.SendLootNotify();
        }

        /// <summary>
        /// Generates loot for the specified player from a looted entity.
        /// </summary>
        /// <param name="looter">The player who looted the entity.</param>
        /// <param name="lootedEntity">The entity that was looted.</param>
        public void GenerateLoot(IPlayer looter, IUnitEntity lootedEntity)
        {
            ILootBuilder lootBuilder = lootGroupManager.GenerateEntityLoot(lootedEntity.CreatureId, [ looter ]);
            if (lootBuilder == null)
                return;

            ILootInstance lootInstance = lootInstanceFactory.Resolve();
            lootInstance.Initialise(this, lootedEntity);

            if (!lootInstances.ContainsKey(lootedEntity.Guid))
                lootInstances.Add(lootedEntity.Guid, []);

            lootInstances[lootedEntity.Guid].Add(lootInstance);

            foreach (ILootBuilderItem lootBuilderItem in lootBuilder.GetItems())
            {
                ILootInstanceItem lootInstanceItem = lootInstance.AddLootItem(lootBuilderItem);
                lootInstanceItem.SetLootable();
            }

            lootInstance.SendLootNotify();
        }

        /// <summary>
        /// Generates loot for the specified group from a looted entity.
        /// </summary>
        /// <param name="group">The group that looted the entity.</param>
        /// <param name="lootedEntity">The entity that was looted.</param>
        public void GenerateLoot(IGroup group, IUnitEntity lootedEntity)
        {
            List<IPlayer> players = [];
            foreach (IGroupMember member in group)
            {
                IPlayer player = lootedEntity.GetVisiblePlayer(member.Identity);
                if (player != null)
                    players.Add(player);
            }

            ILootBuilder lootBuilder = lootGroupManager.GenerateEntityLoot(lootedEntity.CreatureId, players);
            if (lootBuilder == null)
                return;

            ILootInstance lootInstance = lootInstanceFactory.Resolve();
            lootInstance.Initialise(this, lootedEntity);

            if (!lootInstances.ContainsKey(lootedEntity.Guid))
                lootInstances.Add(lootedEntity.Guid, []);

            lootInstances[lootedEntity.Guid].Add(lootInstance);

            foreach (ILootBuilderItem lootBuilderItem in lootBuilder.GetItems())
            {
                ILootInstanceItem lootInstanceItem = lootInstance.AddLootItem(lootBuilderItem);
                if (lootInstanceItem.Type != LootItemType.StaticItem)
                    continue;

                IItemInfo itemInfo = itemManager.GetItemInfo(lootInstanceItem.StaticId);
                if (itemInfo == null)
                    continue;

                LootRule lootRule;
                if (itemInfo.Quality >= group.ThresholdQuality)
                    lootRule = group.ThresholdRule;
                else
                    lootRule = group.NormalRule;

                switch (lootRule)
                {
                    case LootRule.FreeForAll:
                        lootInstanceItem.SetLootable();
                        break;
                    case LootRule.RoundRobin:
                        // TODO:...
                        break;
                    case LootRule.NeedBeforeGreed:
                        lootInstanceItem.SetRollTime(TimeSpan.FromMinutes(5));
                        break;
                    case LootRule.Master:
                        lootInstanceItem.SetMasterLooter(group.Leader);
                        break;
                }
            }

            lootInstance.SendLootNotify();
        }

        /// <summary>
        /// Vacuums all loot in range for the specified player.
        /// </summary>
        /// <param name="player">The player to vacuum loot for.</param>
        public void VacuumLoot(IPlayer player)
        {
            // TODO: might be better use a grid searcher...
            foreach (IGridEntity entity in player.GetVisible())
            {
                if (entity is not IUnitEntity)
                    continue;

                if (!lootInstances.TryGetValue(entity.Guid, out List<ILootInstance> entityLootInstances))
                    continue;

                foreach (ILootInstance lootInstance in entityLootInstances)
                {
                    foreach (ILootInstanceItem lootInstanceItem in lootInstance.GetLootItems())
                    {
                        if (lootInstanceItem.CanLoot)
                            lootInstanceItem.TakeLoot(player);
                    }
                }
            }
        }
    }
}
