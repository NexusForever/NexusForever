using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Loot;
using NexusForever.Game.Abstract.Loot.Generate;
using NexusForever.Game.Static.AccountInventory;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Loot;
using NexusForever.Game.Static.Quest;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Loot;
using NexusForever.Network.World.Message.Static;
using NexusForever.Shared;
using NexusForever.Shared.Game;
using NetworkLootItem = NexusForever.Network.World.Message.Model.Loot.LootItem;

namespace NexusForever.Game.Loot
{
    public class LootInstanceItem : ILootInstanceItem
    {
        public ILootInstance LootInstance { get; private set; }

        public uint Id { get; private set; }
        public uint StaticId { get; private set; }
        public LootItemType Type { get; private set; }
        public uint Count { get; private set; }
        public Identity Winner { get; private set; }
        public bool CanLoot { get; private set; }
        public bool Delivered { get; private set; }

        private readonly HashSet<Identity> looters = [];

        private UpdateTimer rollTimer;
        private Queue<int> uniqueRolls;
        private Dictionary<Identity, ILootInstanceItemRoll> rollActions;

        private HashSet<Identity> masterLooters;

        #region Dependency Injection

        private readonly ILootInstanceIdProvider lootInstanceIdProvider;
        private readonly IFactory<ILootInstanceItemRoll> lootInstanceItemRollFactory;

        public LootInstanceItem(
            ILootInstanceIdProvider lootInstanceIdProvider,
            IFactory<ILootInstanceItemRoll> lootInstanceItemRollFactory)
        {
            this.lootInstanceIdProvider      = lootInstanceIdProvider;
            this.lootInstanceItemRollFactory = lootInstanceItemRollFactory;
        }

        #endregion

        public void Initialise(ILootInstance lootInstance, ILootBuilderItem lootBuilderItem)
        {
            LootInstance = lootInstance;
            Id       = lootInstanceIdProvider.GetId();
            StaticId = lootBuilderItem.StaticId;
            Type     = lootBuilderItem.Type;
            Count    = lootBuilderItem.Count;

            foreach (Identity looter in lootBuilderItem.Looters)
                looters.Add(looter);
        }

        public void Initialise(ILootInstance lootInstance, uint staticId, LootItemType type, uint count, IEnumerable<Identity> looters)
        {
            LootInstance = lootInstance;
            Id       = lootInstanceIdProvider.GetId();
            StaticId = staticId;
            Type     = type;
            Count    = count;

            foreach (Identity looter in looters)
                this.looters.Add(looter);
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            if (rollTimer == null)
                return;

            rollTimer.Update(lastTick);
            if (rollTimer.HasElapsed)
            {
                DecideRollWinner();
                rollTimer = null;
            }
        }

        /// <summary>
        /// Determines if the specified identity is a master looter.
        /// </summary>
        /// <param name="identity">The identity to check.</param>
        /// <returns>
        /// <see langword="true"/> if the identity is a master looter, otherwise <see langword="false"/>.
        /// </returns>
        public bool IsMasterLooter(Identity identity)
        {
            return masterLooters.Contains(identity);
        }

        /// <summary>
        /// Sets the specified identity as a master looter.
        /// </summary>
        /// <remarks>
        /// Retail appears to have supported only a single master looter. However, as the client supports multiple master looters, this implementation allows multiple.
        /// </remarks>
        /// <param name="identity">The identity to set as a master looter.</param>
        public void SetMasterLooter(Identity identity)
        {
            masterLooters ??= [];
            masterLooters.Add(identity);
        }

        /// <summary>
        /// Sets the winner of the loot item.
        /// </summary>
        /// <remarks>
        /// Loot item still needs to be delivered to the winner
        /// </remarks>
        /// <param name="winner">The identity of the winning player.</param>
        public void SetWinner(Identity winner)
        {
            if (Winner != null)
                return;

            Winner = winner;
        }

        /// <summary>
        /// Makes the loot item available for looting.
        /// </summary>
        /// <remarks>
        /// Lootable means players can ether loot or vaccum the item from the ground. This usually occurs for Free For All loot items, or when a loot roll has completed and no winner was determined.
        /// </remarks>
        public void SetLootable()
        {
            if (rollTimer != null)
                return;

            if (masterLooters != null)
                return;

            CanLoot = true;
        }

        /// <summary>
        /// Sets the time for the loot roll.
        /// </summary>
        /// <param name="timeSpan">The time span for the loot roll.</param>
        public void SetRollTime(TimeSpan timeSpan)
        {
            if (rollTimer != null)
                return;

            rollTimer   = new UpdateTimer(timeSpan);
            rollActions = [];

            uniqueRolls = new Queue<int>(
                Enumerable.Range(1, 100)
                    .OrderBy(_ => Random.Shared.Next())
                    .Take(looters.Count));
        }

        /// <summary>
        /// Sets the roll action for the specified identity.
        /// </summary>
        /// <param name="identity">The identity to set the roll action for.</param>
        /// <param name="lootRollAction">The loot roll action to set.</param>
        /// <remarks>
        /// If the roll action is not <see cref="LootRollAction.Pass"/>, a unique roll value will be assigned to the identity. If all looters have set their roll actions, the winner will be decided.
        /// </remarks>
        public void SetRollAction(Identity identity, LootRollAction lootRollAction)
        {
            if (rollTimer == null)
                return;

            if (!looters.Contains(identity))
                return;

            if (rollActions.ContainsKey(identity))
                return;

            ILootInstanceItemRoll roll = lootInstanceItemRollFactory.Resolve();
            roll.Initialise(identity, lootRollAction, lootRollAction != LootRollAction.Pass ? (uint)uniqueRolls.Dequeue() : 0u);
            rollActions[identity] = roll;

            EnqueueMessageToLooters(new ServerLootRoll
            {
                LootUnitId = Id,
                ItemId     = StaticId,
                Roller     = identity.ToNetworkIdentity(),
                Action     = lootRollAction
            });

            if (looters.Count == rollActions.Count)
                DecideRollWinner();
        }

        private void EnqueueMessageToLooters(IWritable message)
        {
            foreach (Identity identity in looters)
            {
                IPlayer player = LootInstance.LootManager.Map.PlayerManager.GetPlayer(identity);
                player?.Session.EnqueueMessageEncrypted(message);
            }
        }

        private void DecideRollWinner()
        {
            ILootInstanceItemRoll winner = null;
            foreach (ILootInstanceItemRoll lootInstanceItemRoll in rollActions.Values)
            {
                if (lootInstanceItemRoll.Action == LootRollAction.Pass)
                    continue;

                if (winner == null)
                    winner = lootInstanceItemRoll;
                else
                {
                    if (winner.Action < lootInstanceItemRoll.Action)
                        continue;

                    if (winner.Roll > lootInstanceItemRoll.Roll)
                        continue;

                    winner = lootInstanceItemRoll;
                }
            }

            var lootWinner = new ServerLootWinner
            {
                LootUnitId = Id,
                ItemId     = StaticId
            };

            if (winner != null)
            {
                lootWinner.WinningRoll = winner.Build();
                lootWinner.OtherRolls  = rollActions.Values
                    .Where(r => r != winner)
                    .Select(r => r.Build())
                    .ToList();

                SetWinner(winner.Identity);

                IPlayer player = LootInstance.LootManager.Map.PlayerManager.GetPlayer(winner.Identity);
                if (player != null)
                    DeliverLoot(player);
                else
                    DeliverLootOffline(winner.Identity);
            }
            else
            {
                lootWinner.WinningRoll = new ServerLootWinner.LootRoll
                {
                    Identity = new Identity
                    {
                        Id      = 0,
                        RealmId = 0
                    }.ToNetworkIdentity()
                };

                SetLootable();
                EnqueueMessageToLooters(new ServerLootCanLoot
                {
                    LootUnitId = Id
                });
            }

            EnqueueMessageToLooters(lootWinner);
        }

        /// <summary>
        /// Takes the loot for the specified player.
        /// </summary>
        /// <param name="player">The player taking the loot.</param>
        /// <remarks>
        /// This method can only be called when the loot has no winner, such as for Free For All loot or when a loot roll completed and no winner was determined.
        /// </remarks>
        public void TakeLoot(IPlayer player)
        {
            if (rollTimer != null)
                return;

            if (!looters.Contains(player.Identity))
                return;

            if (!CanLoot)
                return;

            EnqueueMessageToLooters(new ServerLootGrant
            {
                OwnerUnitId  = LootInstance.OwnerGuid,
                LooterUnitId = player.Guid,
                LootItem     = Build()
            });

            // ServerLootNotification

            SetWinner(player.Identity);
            DeliverLoot(player);
        }

        /// <summary>
        /// Assigns the loot to the specified player.
        /// </summary>
        /// <param name="player">The player to assign the loot to.</param>
        /// <remarks>
        /// This method can only be called when the loot is being assigned by a master looter.
        /// </remarks>
        public void AssignLoot(IPlayer player)
        {
            if (masterLooters == null)
                return;

            if (!looters.Contains(player.Identity))
                return;

            EnqueueMessageToLooters(new ServerLootWinner
            {
                LootUnitId  = Id,
                ItemId      = StaticId,
                WinningRoll = new ServerLootWinner.LootRoll
                {
                    Identity = player.Identity.ToNetworkIdentity(),
                    Value    = -1
                }
            });

            EnqueueMessageToLooters(new ServerLootGrant
            {
                OwnerUnitId  = LootInstance.OwnerGuid,
                LooterUnitId = player.Guid,
                LootItem     = Build()
            });

            SetWinner(player.Identity);
            DeliverLoot(player);
        }

        /// <summary>
        /// Delivers the loot to the specified player.
        /// </summary>
        /// <param name="player">The player to deliver the loot to.</param>
        public void DeliverLoot(IPlayer player)
        {
            if (Delivered)
                return;

            if (Winner != player.Identity)
                return;

            switch (Type)
            {
                case LootItemType.AccountCurrency:
                    player.Account.CurrencyManager.CurrencyAddAmount((AccountCurrencyType)StaticId, Count);
                    // TODO: Send as Notify
                    break;
                case LootItemType.Cash:
                    player.CurrencyManager.CurrencyAddAmount((CurrencyType)StaticId, Count, isLoot: true);
                    break;
                case LootItemType.StaticItem:
                    player.Inventory.ItemCreate(InventoryLocation.Inventory, StaticId, Count, ItemUpdateReason.Loot);
                    break;
                case LootItemType.VirtualItem:
                    player.QuestManager.ObjectiveUpdate(QuestObjectiveType.VirtualCollect, StaticId, Count);
                    break;
                default:
                    //log.LogWarning($"{Type} not supported as deliverable loot.");
                    return;
            }

            Delivered = true;
        }

        /// <summary>
        /// Delivers the loot to the specified identity when the player is offline.
        /// </summary>
        /// <param name="identity">The identity to deliver the loot to.</param>
        public void DeliverLootOffline(Identity identity)
        {
            if (Delivered)
                return;

            if (Winner != identity)
                return;

            // TODO: send mail to offline player with loot item attached

            Delivered = true;
        }

        public NetworkLootItem Build()
        {
            var lootItem = new NetworkLootItem
            {
                LootUnitId        = Delivered ? 0 : Id,
                Type              = Type,
                StaticId          = StaticId,
                Amount            = Count,
                Delivered         = Delivered,
                CanLoot           = CanLoot,
                RandomCircuitData = 0,
                RandomGlyphData   = 0
            };

            if (masterLooters != null)
            {
                lootItem.OnlyMasterLootable = true;
                foreach (Identity masterLooter in masterLooters)
                    lootItem.MasterList.Add(masterLooter.ToNetworkIdentity());
            }

            if (rollTimer != null)
            {
                lootItem.RequiresRoll = true;
                lootItem.RollTime     = (uint)TimeSpan.FromSeconds(rollTimer.Time).TotalMilliseconds;
            }

            return lootItem;
        }
    }
}
