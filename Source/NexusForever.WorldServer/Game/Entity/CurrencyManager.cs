using System;
using System.Collections;
using System.Collections.Generic;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;

namespace NexusForever.WorldServer.Game.Entity
{
    public class CurrencyManager : ISaveCharacter, IEnumerable<Currency>
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly Player player;
        private readonly Dictionary<byte, Currency> currencies = new Dictionary<byte, Currency>();

        /// <summary>
        /// Create a new <see cref="CurrencyManager"/> from <see cref="Player"/> database model.
        /// </summary>
        public CurrencyManager(Player owner, Character model)
        {
            player = owner;

            foreach (var characterCurrency in model.CharacterCurrency)
                currencies.Add(characterCurrency.CurrencyId, new Currency(characterCurrency));
        }

        /// <summary>
        /// Create a new <see cref="CharacterCurrency"/>.
        /// </summary>
        public Currency CurrencyCreate(byte currencyId, ulong amount = 0)
        {
            CurrencyTypeEntry currencyEntry = GameTableManager.CurrencyType.GetEntry(currencyId);
            if (currencyEntry == null)
                return null;

            return CurrencyCreate(currencyEntry);
        }

        /// <summary>
        /// Create a new <see cref="CharacterCurrency"/>.
        /// </summary>
        public Currency CurrencyCreate(CurrencyTypeEntry currencyEntry, ulong amount = 0)
        {
            if (currencyEntry == null)
                return null;

            if (currencies.ContainsKey((byte)currencyEntry.Id))
                throw new ArgumentException($"Currency {currencyEntry.Id} is already added to the player!");

            Currency currency = new Currency(
                player.CharacterId,
                currencyEntry,
                amount
            );
            currencies.Add((byte)currencyEntry.Id, currency);
            return currency;
        }

        /// <summary>
        ///Update <see cref="CharacterCurrency"/> with supplied amount.
        /// </summary>
        private void CurrencyAmountUpdate(Currency currency, ulong amount)
        {
            if (currency == null)
                throw new ArgumentNullException();

            currency.Amount = amount;
            
            player.Session.EnqueueMessageEncrypted(new ServerPlayerCurrencyChanged
            {
                CurrencyId = currency.Id,
                Amount = currency.Amount,
            });
        }

        /// <summary>
        /// Create a new <see cref="CharacterCurrency"/>.
        /// </summary>
        public void CurrencyAddAmount(byte currencyId, ulong amount)
        {
            CurrencyTypeEntry currencyEntry = GameTableManager.CurrencyType.GetEntry(currencyId);
            if (currencyEntry == null)
                throw new ArgumentNullException();

            CurrencyAddAmount(currencyEntry, amount);
        }

        /// <summary>
        /// Create a new <see cref="CharacterCurrency"/>.
        /// </summary>
        public void CurrencyAddAmount(CurrencyTypeEntry currencyEntry, ulong amount)
        {
            if (currencyEntry == null)
                throw new ArgumentNullException();

            if (!currencies.TryGetValue((byte)currencyEntry.Id, out Currency currency))
                CurrencyCreate(currencyEntry, (ulong)amount);
            else
            {
                amount += currency.Amount;
                if (currency.Entry.CapAmount > 0)
                    amount = Math.Min(amount + currency.Amount, currency.Entry.CapAmount);
                CurrencyAmountUpdate(currency, amount);
            }
        }

        /// <summary>
        /// Create a new <see cref="CharacterCurrency"/>.
        /// </summary>
        public void CurrencySubtractAmount(byte currencyId, ulong amount)
        {
            CurrencyTypeEntry currencyEntry = GameTableManager.CurrencyType.GetEntry(currencyId);
            if (currencyEntry == null)
                throw new ArgumentNullException();

            CurrencySubtractAmount(currencyEntry, amount);
        }

        /// <summary>
        /// Create a new <see cref="CharacterCurrency"/>.
        /// </summary>
        public void CurrencySubtractAmount(CurrencyTypeEntry currencyEntry, ulong amount)
        {
            if (currencyEntry == null)
                throw new ArgumentNullException();

            if (!currencies.TryGetValue((byte)currencyEntry.Id, out Currency currency))
                throw new ArgumentException($"Cannot create currency {currencyEntry.Id} with a negative amount!");
            if (currency.Amount < amount)
                throw new ArgumentException($"Trying to remove more currency {currencyEntry.Id} than the player has!");
            CurrencyAmountUpdate(currency, currency.Amount - amount);
        }

        public Currency GetCurrency(uint currencyId)
        {
            return GetCurrency((byte)currencyId);
        }

        public Currency GetCurrency(byte currencyId)
        {
            if (!currencies.TryGetValue(currencyId, out Currency currency))
                return CurrencyCreate(currencyId);
            return currency;
        }

        public bool CanAfford(byte currencyId, ulong amount)
        {
            if (!currencies.TryGetValue(currencyId, out Currency currency))
                return false;

            return currency.Amount >= amount;
        }

        public void Save(CharacterContext context)
        {
            foreach (Currency currency in currencies.Values)
                currency.Save(context);
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
