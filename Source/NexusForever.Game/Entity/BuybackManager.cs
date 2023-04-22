using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;
using NetworkBuybackItem = NexusForever.Network.World.Message.Model.Shared.BuybackItem;

namespace NexusForever.Game.Entity
{
    public sealed class BuybackManager : Singleton<BuybackManager>, IBuybackManager
    {
        private readonly Dictionary<ulong /*CharacterId*/, IBuybackInfo> buybackInfo = new();

        public void Update(double lastTick)
        {
            foreach ((ulong characterId, IBuybackInfo info) in buybackInfo)
            {
                foreach (IBuybackItem buybackItem in info.ToArray())
                {
                    buybackItem.Update(lastTick);
                    if (!buybackItem.HasExpired)
                        continue;

                    info.RemoveItem(buybackItem.UniqueId);

                    IPlayer player = PlayerManager.Instance.GetPlayer(characterId);
                    player.Session?.EnqueueMessageEncrypted(new ServerBuybackItemRemoved
                    {
                        UniqueId = buybackItem.UniqueId
                    });
                }
            }
        }

        /// <summary>
        /// Return stored <see cref="IBuybackItem"/> for <see cref="IPlayer"/> with unique id.
        /// </summary>
        public IBuybackItem GetItem(IPlayer player, uint uniqueId)
        {
            return buybackInfo.TryGetValue(player.CharacterId, out IBuybackInfo info) ? info.GetItem(uniqueId) : null;
        }

        /// <summary>
        /// Create a new <see cref="IBuybackItem"/> from sold <see cref="IItem"/> for <see cref="IPlayer"/>.
        /// </summary>
        public void AddItem(IPlayer player, IItem item, uint quantity, List<(CurrencyType CurrencyTypeId, ulong CurrencyAmount)> currencyChange)
        {
            if (!buybackInfo.ContainsKey(player.CharacterId))
                buybackInfo.Add(player.CharacterId, new BuybackInfo());

            uint uniqueId = buybackInfo[player.CharacterId].AddItem(item, quantity, currencyChange);

            var networkBuybackItem = new NetworkBuybackItem
            {
                UniqueId = uniqueId,
                ItemId = item.Info.Id,
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
        /// Remove stored <see cref="IBuybackItem"/> with supplied unique id for <see cref="IPlayer"/>.
        /// </summary>
        public void RemoveItem(IPlayer player, IBuybackItem item)
        {
            if (!buybackInfo.TryGetValue(player.CharacterId, out IBuybackInfo info))
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
        /// Send <see cref="ServerBuybackItems"/> for <see cref="IPlayer"/>.
        /// </summary>
        public void SendBuybackItems(IPlayer player)
        {
            if (!buybackInfo.TryGetValue(player.CharacterId, out IBuybackInfo info))
                return;

            var serverBuybackItems = new ServerBuybackItems();
            foreach (IBuybackItem buybackItem in info)
                serverBuybackItems.BuybackItems.Add(buybackItem.Build());

            player.Session.EnqueueMessageEncrypted(serverBuybackItems);
        }
    }
}
