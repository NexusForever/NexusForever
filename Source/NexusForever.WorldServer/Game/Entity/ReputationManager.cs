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
    public class ReputationManager : ISaveCharacter, IEnumerable<Reputation>
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly Player player;
        private readonly Dictionary<uint, Reputation> reputations = new Dictionary<uint, Reputation>();

        /// <summary>
        /// Create a new <see cref="ReputationManager"/> from <see cref="Player"/> database model.
        /// </summary>
        public ReputationManager(Player owner, Character model)
        {
            player = owner;

            foreach (var characterReputation in model.CharacterReputation)
                reputations.Add(characterReputation.FactionId, new Reputation(characterReputation));
        }

        /// <summary>
        /// Create a new <see cref="CharacterReputation"/>.
        /// </summary>
        public Reputation ReputationCreate(uint factionId, ulong value = 0)
        {
            Faction2Entry reputationEntry = GameTableManager.Faction2.GetEntry(factionId);
            if (reputationEntry == null)
                return null;

            return ReputationCreate(reputationEntry);
        }

        /// <summary>
        /// Create a new <see cref="CharacterReputation"/>.
        /// </summary>
        public Reputation ReputationCreate(Faction2Entry reputationEntry, ulong value = 0)
        {
            if (reputationEntry == null)
            {
                log.Info("reputatioEntry == null");
                return null;
            }

            if (reputations.ContainsKey((uint)reputationEntry.Id))
                throw new ArgumentException($"Reputation {reputationEntry.Id} is already added to the player!");

            Reputation reputation = new Reputation(
                player.CharacterId,
                reputationEntry,
                value
            );
            reputations.Add((uint)reputationEntry.Id, reputation);
            log.Info($"Adding {reputationEntry.Id}, {reputationEntry.Id}");
            return reputation;
        }

        /// <summary>
        ///Update <see cref="CharacterCurrency"/> with supplied amount.
        /// </summary>
        //private void CurrencyAmountUpdate(Currency currency, ulong amount)
        //{
        //    if (currency == null)
        //        throw new ArgumentNullException();

        //    currency.Amount = amount;

        //    player.Session.EnqueueMessageEncrypted(new ServerPlayerCurrencyChanged
        //    {
        //        CurrencyId = (byte)currency.Id,
        //        Amount = currency.Amount,
        //    });
        //}

        /// <summary>
        /// Create a new <see cref="CharacterReputation"/>.
        /// </summary>
        public void ReputationAddValue(uint reputationId, ulong value)
        {
            Faction2Entry reputationEntry = GameTableManager.Faction2.GetEntry(reputationId);
            if (reputationEntry == null)
                throw new ArgumentNullException();

            ReputationAddValue(reputationEntry, value);
        }

        /// <summary>
        /// Create a new <see cref="CharacterReputation"/>.
        /// </summary>
        public void ReputationAddValue(Faction2Entry reputationEntry, ulong value)
        {
            if (reputationEntry == null)
                throw new ArgumentNullException();

            if (!reputations.TryGetValue((byte)reputationEntry.Id, out Reputation reputation))
                ReputationCreate(reputationEntry, (ulong)value);
            else
            {
                value += reputation.Value;
                // TODO: Update this to check for capped faction
                //if (reputation.Entry.CapAmount > 0)
                //    amount = Math.Min(amount + currency.Amount, currency.Entry.CapAmount);
                //CurrencyAmountUpdate(currency, amount);
            }
        }

        ///// <summary>
        ///// Create a new <see cref="CharacterCurrency"/>.
        ///// </summary>
        //public void CurrencySubtractAmount(byte currencyId, ulong amount)
        //{
        //    CurrencyTypeEntry currencyEntry = GameTableManager.CurrencyType.GetEntry(currencyId);
        //    if (currencyEntry == null)
        //        throw new ArgumentNullException();

        //    CurrencySubtractAmount(currencyEntry, amount);
        //}

        ///// <summary>
        ///// Create a new <see cref="CharacterCurrency"/>.
        ///// </summary>
        //public void CurrencySubtractAmount(CurrencyTypeEntry currencyEntry, ulong amount)
        //{
        //    if (currencyEntry == null)
        //        throw new ArgumentNullException();

        //    if (!currencies.TryGetValue((byte)currencyEntry.Id, out Currency currency))
        //        throw new ArgumentException($"Cannot create currency {currencyEntry.Id} with a negative amount!");
        //    if (currency.Amount < amount)
        //        throw new ArgumentException($"Trying to remove more currency {currencyEntry.Id} than the player has!");
        //    CurrencyAmountUpdate(currency, currency.Amount - amount);
        //}

        //public Currency GetCurrency(uint currencyId)
        //{
        //    return GetCurrency((byte)currencyId);
        //}

        public Reputation GetReputation(uint reputationId)
        {
            if (!reputations.TryGetValue(reputationId, out Reputation reputation))
                return ReputationCreate(reputationId);
            return reputation;
        }

        public void Save(CharacterContext context)
        {
            foreach (Reputation reputation in reputations.Values)
                reputation.Save(context);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Reputation> GetEnumerator()
        {
            return reputations.Values.GetEnumerator();
        }
    }
}
