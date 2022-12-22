using NexusForever.Game.Network;
using NexusForever.Game.Static.Entity;
using NexusForever.Network;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;
using NetworkBuybackItem = NexusForever.Network.World.Message.Model.Shared.BuybackItem;

namespace NexusForever.Game.Entity
{
    public sealed class BuybackManager : Singleton<BuybackManager>, IUpdate
    {
        private readonly Dictionary<ulong /*CharacterId*/, BuybackInfo> buybackInfo = new();

        private BuybackManager()
        {
        }

        public void Update(double lastTick)
        {
            foreach ((ulong characterId, BuybackInfo info) in buybackInfo)
            {
                foreach (BuybackItem buybackItem in info.ToArray())
                {
                    buybackItem.Update(lastTick);
                    if (!buybackItem.HasExpired)
                        continue;

                    info.RemoveItem(buybackItem.UniqueId);

                    // TODO: probably need a better way to handle this
                    WorldSession session = NetworkManager<WorldSession>.Instance.GetSession(s => s.Player?.CharacterId == characterId);
                    session?.EnqueueMessageEncrypted(new ServerBuybackItemRemoved
                    {
                        UniqueId = buybackItem.UniqueId
                    });
                }
            }
        }

        /// <summary>
        /// Return stored <see cref="BuybackItem"/> for <see cref="Player"/> with unique id.
        /// </summary>
        public BuybackItem GetItem(Player player, uint uniqueId)
        {
            return buybackInfo.TryGetValue(player.CharacterId, out BuybackInfo info) ? info.GetItem(uniqueId) : null;
        }

        /// <summary>
        /// Create a new <see cref="BuybackItem"/> from sold <see cref="Item"/> for <see cref="Player"/>.
        /// </summary>
        public void AddItem(Player player, Item item, uint quantity, List<(CurrencyType CurrencyTypeId, ulong CurrencyAmount)> currencyChange)
        {
            if (!buybackInfo.ContainsKey(player.CharacterId))
                buybackInfo.Add(player.CharacterId, new BuybackInfo());

            uint uniqueId = buybackInfo[player.CharacterId].AddItem(item, quantity, currencyChange);

            var networkBuybackItem = new NetworkBuybackItem
            {
                UniqueId = uniqueId,
                ItemId   = item.Info.Id,
                Quantity = quantity
            };

            for (int i = 0; i < networkBuybackItem.CurrencyTypeId.Length; i++)
            {
                if (i >= currencyChange.Count)
                    continue;

                networkBuybackItem.CurrencyTypeId[i] = currencyChange[i].CurrencyTypeId;
                networkBuybackItem.CurrencyAmount[i] = currencyChange[i].CurrencyAmount;
            }

            player.Session.EnqueueMessageEncrypted(new ServerBuybackItemUpdated
            {
                BuybackItem = networkBuybackItem
            });
        }

        /// <summary>
        /// Remove stored <see cref="BuybackItem"/> with supplied unique id for <see cref="Player"/>.
        /// </summary>
        public void RemoveItem(Player player, BuybackItem item)
        {
            if (!buybackInfo.TryGetValue(player.CharacterId, out BuybackInfo info))
                return;

            info.RemoveItem(item.UniqueId);
            if (!info.Any())
                buybackInfo.Remove(player.CharacterId);

            player.Session.EnqueueMessageEncrypted(new ServerBuybackItemRemoved
            {
                UniqueId = item.UniqueId
            });
        }

        /// <summary>
        /// Send <see cref="ServerBuybackItems"/> for <see cref="Player"/>.
        /// </summary>
        public void SendBuybackItems(Player player)
        {
            if (!buybackInfo.TryGetValue(player.CharacterId, out BuybackInfo info))
                return;

            var serverBuybackItems = new ServerBuybackItems();
            foreach (BuybackItem buybackItem in info)
            {
                var networkBuybackItem = new NetworkBuybackItem
                {
                    UniqueId = buybackItem.UniqueId,
                    ItemId   = buybackItem.Item.Info.Id,
                    Quantity = buybackItem.Quantity,
                };

                for (int i = 0; i < networkBuybackItem.CurrencyTypeId.Length; i++)
                {
                    if (i >= buybackItem.CurrencyChange.Count)
                        continue;

                    networkBuybackItem.CurrencyTypeId[i] = buybackItem.CurrencyChange[i].CurrencyTypeId;
                    networkBuybackItem.CurrencyAmount[i] = buybackItem.CurrencyChange[i].CurrencyAmount;
                }

                serverBuybackItems.BuybackItems.Add(networkBuybackItem);
            }

            player.Session.EnqueueMessageEncrypted(serverBuybackItems);
        }
    }
}
