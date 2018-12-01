using System;
using System.Collections.Generic;
using NexusForever.Shared;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;

namespace NexusForever.WorldServer.Game.Entity
{
    public class CurrencyManager : IUpdate, ISaveCharacter
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly ulong characterId;
        private readonly Player player;
        private readonly Dictionary<byte, Currency> currencies = new Dictionary<byte, Currency>();

        /// <summary>
        /// Create a new <see cref="CurrencyManager"/> from <see cref="Player"/> database model.
        /// </summary>
        public CurrencyManager(Player owner, Character model)
        {
            characterId = owner?.CharacterId ?? 0ul;
            player = owner;

            foreach (var characterCurrency in model.CharacterCurrency)
                currencies.Add(characterCurrency.CurrencyId, new Currency(characterCurrency));
        }

        public void Update(double lastTick)
        {
            // TODO: tick items with limited lifespans
        }

        /// <summary>
        /// Create a new <see cref="CharacterCurrency"/>.
        /// </summary>
        public Currency CurrencyCreate(byte currencyId, ulong count = 0)
        {
            CurrencyTypeEntry currencyEntry = GameTableManager.CurrencyType.GetEntry(currencyId);
            if (currencyEntry == null)
                return null;

            return CurrencyCreate(currencyEntry);
        }

        /// <summary>
        /// Create a new <see cref="CharacterCurrency"/>.
        /// </summary>
        public Currency CurrencyCreate(CurrencyTypeEntry currencyEntry, ulong count = 0)
        {
            if (currencyEntry == null)
                return null;

            if (currencies.ContainsKey((byte)currencyEntry.Id))
                throw new ArgumentException($"Currency {currencyEntry.Id} is already added to the player!");

            Currency currency = new Currency(
                player.CharacterId,
                currencyEntry,
                count
            );
            currencies.Add((byte)currencyEntry.Id, currency);
            return currency;
        }

        /// <summary>
        ///Update <see cref="CharacterCurrency"/> with supplied count.
        /// </summary>
        private void CurrencyCountUpdate(Currency currency, ulong count)
        {
            if (currency == null)
                throw new ArgumentNullException();

            currency.Count = count;
            
            player.Session.EnqueueMessageEncrypted(new ServerPlayerCurrencyChanged
            {
                CurrencyId = (byte)currency.Id,
                Count = currency.Count,
            });
        }

        /// <summary>
        /// Create a new <see cref="CharacterCurrency"/>.
        /// </summary>
        public void CurrencyAddCount(byte currencyId, ulong count)
        {
            CurrencyTypeEntry currencyEntry = GameTableManager.CurrencyType.GetEntry(currencyId);
            if (currencyEntry == null)
                throw new ArgumentNullException();

            CurrencyAddCount(currencyEntry, count);
        }

        /// <summary>
        /// Create a new <see cref="CharacterCurrency"/>.
        /// </summary>
        public void CurrencyAddCount(CurrencyTypeEntry currencyEntry, ulong count)
        {
            if (currencyEntry == null)
                throw new ArgumentNullException();

            if (!currencies.TryGetValue((byte)currencyEntry.Id, out Currency currency))
                CurrencyCreate(currencyEntry, (ulong)count);
            else
            {
                count += currency.Count;
                if (currency.Entry.CapAmount > 0)
                    count = Math.Min(count + currency.Count, currency.Entry.CapAmount);
                CurrencyCountUpdate(currency, count);
            }
        }

        /// <summary>
        /// Create a new <see cref="CharacterCurrency"/>.
        /// </summary>
        public void CurrencySubtractCount(byte currencyId, ulong count)
        {
            CurrencyTypeEntry currencyEntry = GameTableManager.CurrencyType.GetEntry(currencyId);
            if (currencyEntry == null)
                throw new ArgumentNullException();

            CurrencySubtractCount(currencyEntry, count);
        }

        /// <summary>
        /// Create a new <see cref="CharacterCurrency"/>.
        /// </summary>
        public void CurrencySubtractCount(CurrencyTypeEntry currencyEntry, ulong count)
        {
            if (currencyEntry == null)
                throw new ArgumentNullException();

            if (!currencies.TryGetValue((byte)currencyEntry.Id, out Currency currency))
                throw new ArgumentException($"Cannot create currency {currencyEntry.Id} with a negative count!");
            else if (currency.Count < count)
                throw new ArgumentException($"Trying to remove more currency {currencyEntry.Id} than the player has!");
            CurrencyCountUpdate(currency, currency.Count - count);
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
        
        public IEnumerator<Currency> GetEnumerator()
        {
            return currencies.Values.GetEnumerator();
        }

        public void Save(CharacterContext context)
        {
            foreach (Currency currency in currencies.Values)
                currency.Save(context);
        }
    }
}
