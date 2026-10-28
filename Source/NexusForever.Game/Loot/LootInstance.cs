using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Loot;
using NexusForever.Game.Abstract.Loot.Generate;
using NexusForever.Game.Static.Loot;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Loot;
using NexusForever.Shared;
using NexusForever.Shared.Game;
using NetworkLootItem = NexusForever.Network.World.Message.Model.Loot.LootItem;

namespace NexusForever.Game.Loot
{
    public class LootInstance : ILootInstance
    {
        public ILootManager LootManager { get; private set; }
        public uint OwnerGuid { get; private set; }
        public bool PersonalLoot { get; private set; }
        public bool Expired => lootItems.Count == 0;

        private readonly UpdateTimer expiryTimer = new(TimeSpan.FromMinutes(10));

        private readonly HashSet<Identity> looters = [];
        private readonly Dictionary<uint, ILootInstanceItem> lootItems = [];

        #region Dependency Injection

        private readonly IFactory<ILootInstanceItem> lootInstanceItemFactory;

        public LootInstance(
            IFactory<ILootInstanceItem> lootInstanceItemFactory)
        {
            this.lootInstanceItemFactory = lootInstanceItemFactory;
        }

        #endregion

        public void Initialise(ILootManager lootManager, IUnitEntity lootOwner)
        {
            LootManager  = lootManager;
            OwnerGuid    = lootOwner.Guid;
            PersonalLoot = lootOwner is IPlayer; // TODO: might need additional flexibility
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            expiryTimer.Update(lastTick);
            if (expiryTimer.HasElapsed)
                lootItems.Clear();

            foreach ((uint lootId, ILootInstanceItem lootInstanceItem) in lootItems)
            {
                lootInstanceItem.Update(lastTick);
                if (lootInstanceItem.Delivered)
                    lootItems.Remove(lootId);
            }
        }

        /// <summary>
        /// Gets all loot items associated with this loot instance.
        /// </summary>
        /// <returns>
        /// The loot items associated with this loot instance, or an empty collection if none exist.
        /// </returns>
        public IEnumerable<ILootInstanceItem> GetLootItems()
        {
            return lootItems.Values;
        }

        /// <summary>
        /// Gets the loot item instance for the specified loot id.
        /// </summary>
        /// <param name="lootId">The id of the loot item to retrieve.</param>
        /// <returns>
        /// The loot item instance, or null if not found.
        /// </returns>
        public ILootInstanceItem GetLootItem(uint lootId)
        {
            return lootItems.TryGetValue(lootId, out ILootInstanceItem lootInstanceItem) ? lootInstanceItem : null;
        }

        /// <summary>
        /// Adds a loot item to this loot instance.
        /// </summary>
        /// <param name="lootBuilderItem">The loot builder used to create the loot item.</param>
        /// <returns>
        /// The created loot item instance.
        /// </returns>
        public ILootInstanceItem AddLootItem(ILootBuilderItem lootBuilderItem)
        {
            ILootInstanceItem lootInstanceItem = lootInstanceItemFactory.Resolve();
            lootInstanceItem.Initialise(this, lootBuilderItem);
            lootItems.Add(lootInstanceItem.Id, lootInstanceItem);

            foreach (Identity looter in lootBuilderItem.Looters)
                looters.Add(looter);

            return lootInstanceItem;
        }

        /// <summary>
        /// Adds a loot item to this loot instance.
        /// </summary>
        /// <param name="staticId">The static id of the loot item, this will vary depending on the <paramref name="type"/>.</param>
        /// <param name="type">The type of the loot item.</param>
        /// <param name="count">The count of the loot item.</param>
        /// <param name="looters">The list of players who can loot the item.</param>
        /// <returns>
        /// The created loot item instance.
        /// </returns>
        public ILootInstanceItem AddLootItem(uint staticId, LootItemType type, uint count, IEnumerable<Identity> looters)
        {
            ILootInstanceItem lootInstanceItem = lootInstanceItemFactory.Resolve();
            lootInstanceItem.Initialise(this, staticId, type, count, looters);
            lootItems.Add(lootInstanceItem.Id, lootInstanceItem);

            foreach (Identity looter in looters)
                this.looters.Add(looter);

            return lootInstanceItem;
        }

        /// <summary>
        /// Sends a loot notification to all players who can loot the items from this loot instance.
        /// </summary>
        public void SendLootNotify()
        {
            var networkLootItems = new List<NetworkLootItem>();

            foreach (ILootInstanceItem item in lootItems.Values)
            {
                // TODO: there might be a better way to handle this...
                if (item.Type == LootItemType.AccountCurrency && item.Delivered)
                {
                    uint remaining = item.Count;
                    while (remaining > 0)
                    {
                        NetworkLootItem networkLootItem = item.Build();
                        networkLootItem.Amount = Math.Min(remaining, 50);
                        networkLootItems.Add(networkLootItem);

                        remaining -= networkLootItem.Amount;
                    }
                }
                else
                {
                    NetworkLootItem networkLootItem = item.Build();
                    networkLootItems.Add(networkLootItem);
                }
            }

            EnqueueMessageToLooters(new ServerLootNotify
            {
                OwnerUnitId = OwnerGuid,
                Explosion   = PersonalLoot,
                LootItems   = networkLootItems
            });
        }

        /// <summary>
        /// Sends a loot removal message to all players who can loot the items from this loot instance.
        /// </summary>
        public void SendLootRemove()
        {
            EnqueueMessageToLooters(new ServerLootRemove
            {
                OwnerUnitId = OwnerGuid
            });
        }

        private void EnqueueMessageToLooters(IWritable message)
        {
            foreach (Identity identity in looters)
            {
                IPlayer player = LootManager.Map.PlayerManager.GetPlayer(identity);
                player?.Session.EnqueueMessageEncrypted(message);
            }
        }
    }
}
