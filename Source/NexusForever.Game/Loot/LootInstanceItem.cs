using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Loot;
using NexusForever.Game.Static.Account;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Loot;
using NexusForever.Game.Static.Quest;
using NexusForever.Network.Session;
using NexusForever.Network.World.Message.Model.Loot;
using NexusForever.Network.World.Message.Static;
using NLog;
using NetworkLootItem = NexusForever.Network.World.Message.Model.Loot.LootItem;

namespace NexusForever.Game.Loot
{
    public class LootInstanceItem : ILootInstanceItem
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Loot Item ID the client uses to determine the instance.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Guid of the "looted" entity. This is the Player Guid if the LootItemType is Item.
        /// </summary>
        public uint Guid { get; private set; }

        public uint StaticId { get; }
        public LootItemType Type { get; }
        public uint Amount { get; private set; }

        public uint WinnerGuid { get; private set; }
        public ulong WinnerCharacterId { get; private set; }
        public bool Delivered { get; private set; }

        /// <summary>
        /// Create a new <see cref="LootInstanceItem"/>.
        /// </summary>
        public LootInstanceItem(uint staticId, LootItemType type, uint count)
        {
            Id = GlobalLootManager.Instance.NextLootId;
            StaticId = staticId;
            Type = type;
            Amount = count;

            if (type == LootItemType.StaticItem)
            {
                // TODO: Generate Random Stats and store them if item can have stats
            }
        }

        public void AddToAmount(uint amount)
        {
            Amount += amount;
        }

        /// <summary>
        /// Sets the Winner for this <see cref="LootInstanceItem"/>.
        /// </summary>
        public void SetWinner(ulong characterId, uint guid)
        {
            WinnerGuid = guid;
            WinnerCharacterId = characterId;
        }

        /// <summary>
        /// Sets the UnitId that this <see cref="LootInstanceItem"/> is looted from.
        /// </summary>
        public void SetLootUnit(uint guid)
        {
            Guid = guid;
        }

        /// <summary>
        /// Generates and delivers the <see cref="LootItemType"/> to the given <see cref="WorldSession"/>. If sendAsGrant is unspecified or true, this will also send the <see cref="ServerLootGrant"/> packet to the <see cref="WorldSession"/>.
        /// </summary>
        public void DeliverItem(IPlayer player, bool sendAsGrant = true)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            if (WinnerGuid == 0u || WinnerCharacterId == 0u)
                throw new InvalidOperationException("Winner IDs must be set before delivering this loot item.");

            if (player.CharacterId != WinnerCharacterId)
                throw new InvalidOperationException($"Winner CharacterID {WinnerCharacterId} does not match the provided WorldSession CharacterID {player.CharacterId}.");

            if (Delivered)
                throw new InvalidOperationException($"Loot item has already been delivered to the Winner!");

            switch (Type)
            {
                case LootItemType.AccountCurrency:
                    player.Account.CurrencyManager.CurrencyAddAmount((AccountCurrencyType)StaticId, Amount);
                    // TODO: Send as Notify
                    break;
                case LootItemType.Cash:
                    player.CurrencyManager.CurrencyAddAmount((CurrencyType)StaticId, Amount, isLoot: true);
                    break;
                case LootItemType.StaticItem:
                    player.Inventory.ItemCreate(InventoryLocation.Inventory, StaticId, Amount, ItemUpdateReason.Loot);
                    break;
                case LootItemType.VirtualItem:
                    player.QuestManager.ObjectiveUpdate(QuestObjectiveType.VirtualCollect, StaticId, Amount);
                    break;
                default:
                    log.Warn($"{Type} not supported as deliverable loot.");
                    return;
            }

            Delivered = true;

            if (sendAsGrant)
                SendGrant(player.Session);
        }

        /// <summary>
        /// Sends a <see cref="ServerLootGrant"/> packet for this <see cref="LootInstanceItem"/> to the <see cref="WorldSession"/>.
        /// </summary>
        private void SendGrant(IGameSession session)
        {
            session.EnqueueMessageEncrypted(new ServerLootGrant
            {
                OwnerUnitId  = Guid,
                LooterUnitId = WinnerGuid,
                LootItem     = Build()
            });
        }

        /// <summary>
        /// Builds this <see cref="LootInstanceItem"/> into a <see cref="NetworkLootItem"/> to be sent to a <see cref="WorldSession"/>.
        /// </summary>
        public NetworkLootItem Build()
        {
            return new NetworkLootItem
            {
                LootUnitId = (uint)Id,
                Type = Type,
                StaticId = StaticId,
                Amount = Amount,
                RandomCircuitData = 0,
                RandomGlyphData = 0,
            };
        }

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> containing up to 50 <see cref="NetworkLootItem"/>. Should only be used when the <see cref="LootItemType"/> is an Account Currency.
        /// </summary>
        /// <remarks>This gives the Player a "loot shower" effect when they receive a reward of omnibits or other Account Currencies.</remarks>
        public IEnumerable<NetworkLootItem> BuildForAccountCurrency()
        {
            List<NetworkLootItem> lootItems = new List<NetworkLootItem>();

            uint remainder = Amount > 50 ? Amount % 50 : 0;
            uint totalItems = Amount > 50 ? 50 : Amount;
            for (int i = 0; i < totalItems; i++)
            {
                NetworkLootItem lootItem = new NetworkLootItem
                {
                    LootUnitId = 0,
                    Type = Type,
                    StaticId = StaticId,
                    Amount = Amount > 50 ? Amount % 50 : 1,
                    CanLoot = true,
                    // TODO: no idea what the replacement is after the packet changes...
                    // Granted = true
                };

                if (i == 0 && remainder > 0)
                    lootItem.Amount += remainder;

                lootItems.Add(lootItem);
            }

            return lootItems;
        }

        /// <summary>
        /// Send the <see cref="LootInstanceItem"/> to the Winner if they're offline via mail.
        /// </summary>
        public void DeliverItemOffline()
        {
            throw new NotImplementedException();
        }
    }
}
