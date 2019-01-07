﻿using System.Collections.Generic;
using System.Linq;
using NexusForever.Shared.Network;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using NetworkBuybackItem = NexusForever.WorldServer.Network.Message.Model.Shared.BuybackItem;

namespace NexusForever.WorldServer.Game.Entity
{
    public static class BuybackManager
    {
        private static readonly Dictionary<ulong /*CharacterId*/, BuybackInfo> buybackInfo = new Dictionary<ulong, BuybackInfo>();

        /// <summary>
        /// Return stored <see cref="BuybackItem"/> for <see cref="Player"/> with unique id.
        /// </summary>
        public static BuybackItem GetItem(Player player, uint uniqueId)
        {
            return buybackInfo.TryGetValue(player.CharacterId, out BuybackInfo info) ? info.GetItem(uniqueId) : null;
        }

        /// <summary>
        /// Create a new <see cref="BuybackItem"/> from sold <see cref="Item"/> for <see cref="Player"/>.
        /// </summary>
        public static void AddItem(Player player, Item item, uint quantity, List<(byte CurrencyTypeId, ulong CurrencyAmount)> currencyChange)
        {
            if (!buybackInfo.ContainsKey(player.CharacterId))
                buybackInfo.Add(player.CharacterId, new BuybackInfo());

            uint uniqueId = buybackInfo[player.CharacterId].AddItem(item, quantity, currencyChange);

            var networkBuybackItem = new NetworkBuybackItem
            {
                UniqueId = uniqueId,
                ItemId   = item.Entry.Id,
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
        public static void RemoveItem(Player player, BuybackItem item)
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
        public static void SendBuybackItems(Player player)
        {
            if (!buybackInfo.TryGetValue(player.CharacterId, out BuybackInfo info))
                return;

            var serverBuybackItems = new ServerBuybackItems();
            foreach (BuybackItem buybackItem in info)
            {
                var networkBuybackItem = new NetworkBuybackItem
                {
                    UniqueId = buybackItem.UniqueId,
                    ItemId   = buybackItem.Item.Entry.Id,
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

        public static void Update(double lastTick)
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
                    WorldSession session = NetworkManager<WorldSession>.GetSession(s => s.Player?.CharacterId == characterId);
                    session?.EnqueueMessageEncrypted(new ServerBuybackItemRemoved
                    {
                        UniqueId = buybackItem.UniqueId
                    });
                }
            }
        }
    }
}
