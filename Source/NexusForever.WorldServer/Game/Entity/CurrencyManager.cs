using System;
using System.Collections;
using System.Collections.Generic;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Entity
{
    public class CurrencyManager : ISaveCharacter, IEnumerable<Currency>
    {
        private readonly Player player;
        private readonly Dictionary<CurrencyType, Currency> currencies = new Dictionary<CurrencyType, Currency>();

        /// <summary>
        /// Create a new <see cref="CurrencyManager"/> from <see cref="CharacterModel"/> database model.
        /// </summary>
        public CurrencyManager(Player owner, CharacterModel model)
        {
            player = owner;

            foreach (CharacterCurrencyModel currencyModel in model.Currency)
            {
                var currency = new Currency(currencyModel);
                currencies.Add(currency.Id, currency);
            }
        }

        public void Save(CharacterContext context)
        {
            foreach (Currency currency in currencies.Values)
                currency.Save(context);
        }

        /// <summary>
        /// Returns total amount of acquired <see cref="CurrencyType"/>.
        /// </summary>
        public ulong? GetCurrency(CurrencyType currencyId)
        {
            if (!currencies.TryGetValue(currencyId, out Currency currency))
                return null;
            return currency.Amount;
        }

        /// <summary>
        /// Returns if <see cref="Player"/> has the supplied amount of <see cref="CurrencyType"/>.
        /// </summary>
        public bool CanAfford(CurrencyType currencyId, ulong amount)
        {
            return GetCurrency(currencyId) >= amount;
        }

        /// <summary>
        /// Add supplied amount of <see cref="CurrencyType"/> currency.
        /// </summary>
        public void CurrencyAddAmount(CurrencyType currencyId, ulong amount, bool isLoot = false)
        {
            CurrencyTypeEntry currencyEntry = GameTableManager.Instance.CurrencyType.GetEntry((ulong)currencyId);
            if (currencyEntry == null)
                throw new ArgumentNullException();

            CurrencyAddAmount(currencyEntry, amount, isLoot);
        }

        private void CurrencyAddAmount(CurrencyTypeEntry currencyEntry, ulong amount, bool isLoot = false)
        {
            if (currencyEntry == null)
                throw new ArgumentNullException();

            if (!currencies.TryGetValue((CurrencyType)currencyEntry.Id, out Currency currency))
                currency = CurrencyCreate(currencyEntry);

            amount += currency.Amount;
            if (currency.Entry.CapAmount > 0)
                amount = Math.Min(amount, currency.Entry.CapAmount);

            CurrencyAmountUpdate(currency, amount, isLoot);
        }

        private Currency CurrencyCreate(CurrencyTypeEntry currencyEntry)
        {
            if (currencyEntry == null)
                return null;

            if (currencies.ContainsKey((CurrencyType)currencyEntry.Id))
                throw new ArgumentException($"Currency {currencyEntry.Id} is already added to the player!");

            var currency = new Currency(player.CharacterId, currencyEntry);
            currencies.Add(currency.Id, currency);
            return currency;
        }

        /// <summary>
        /// Remove supplied amount of <see cref="CurrencyType"/> currency.
        /// </summary>
        public void CurrencySubtractAmount(CurrencyType currencyId, ulong amount, bool isLoot = false)
        {
            CurrencyTypeEntry currencyEntry = GameTableManager.Instance.CurrencyType.GetEntry((ulong)currencyId);
            if (currencyEntry == null)
                throw new ArgumentNullException();

            CurrencySubtractAmount(currencyEntry, amount, isLoot);
        }

        private void CurrencySubtractAmount(CurrencyTypeEntry currencyEntry, ulong amount, bool isLoot = false)
        {
            if (currencyEntry == null)
                throw new ArgumentNullException();

            if (!currencies.TryGetValue((CurrencyType)currencyEntry.Id, out Currency currency))
                throw new ArgumentException($"Cannot create currency {currencyEntry.Id} with a negative amount!");
            if (currency.Amount < amount)
                throw new ArgumentException($"Trying to remove more currency {currencyEntry.Id} than the player has!");

            CurrencyAmountUpdate(currency, currency.Amount - amount, isLoot);
        }

        /// <summary>
        /// Update <see cref="Currency"/> with supplied amount of currency.
        /// </summary>
        private void CurrencyAmountUpdate(Currency currency, ulong amount, bool isLoot = false)
        {
            if (currency == null)
                throw new ArgumentNullException();

            if (isLoot)
            {
                player.Session.EnqueueMessageEncrypted(new ServerChannelUpdateLoot
                {
                    CurrencyId = currency.Id,
                    Amount     = amount - currency.Amount
                });
            }

            currency.Amount = amount;

            player.Session.EnqueueMessageEncrypted(new ServerCombatReward
            {
                Stat     = (byte)(currency.Id - 1),
                NewValue = currency.Amount
            });
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Currency> GetEnumerator()
        {
            return currencies.Values.GetEnumerator();
        }
    }
}
