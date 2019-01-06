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
        public Reputation ReputationCreate(uint factionId, uint value = 0)
        {
            Faction2Entry reputationEntry = GameTableManager.Faction2.GetEntry(factionId);
            if (reputationEntry == null)
                return null;

            return ReputationCreate(reputationEntry);
        }

        /// <summary>
        /// Create a new <see cref="CharacterReputation"/>.
        /// </summary>
        public Reputation ReputationCreate(Faction2Entry reputationEntry, uint value = 0)
        {
            if (reputationEntry == null)
                return null;

            if (reputations.ContainsKey((uint)reputationEntry.Id))
                throw new ArgumentException($"Reputation {reputationEntry.Id} is already added to the player!");

            Reputation reputation = new Reputation(
                player.CharacterId,
                reputationEntry,
                value
            );
            reputations.Add(reputationEntry.Id, reputation);
            ReputationValueUpdate(reputation, value);
            return reputation;
        }

        /// <summary>
        /// Create a new <see cref="CharacterReputation"/>.
        /// </summary>
        public void AddValue(uint reputationId, uint value)
        {
            Faction2Entry reputationEntry = GameTableManager.Faction2.GetEntry(reputationId);
            if (reputationEntry == null)
                throw new ArgumentNullException();

            AddValue(reputationEntry, value);
        }

        /// <summary>
        /// Create a new <see cref="CharacterReputation"/>.
        /// </summary>
        public void AddValue(Faction2Entry reputationEntry, uint value)
        {
            if (reputationEntry == null)
                throw new ArgumentNullException();

            if (!reputations.TryGetValue(reputationEntry.Id, out Reputation reputation))
                ReputationCreate(reputationEntry, value);
            else
            {
                float valueToSend = 0f;
                if (reputation.Amount + value > 32000)
                    valueToSend = 32000 - reputation.Amount;
                else
                    valueToSend = value;

                if (valueToSend > 0)
                {
                    reputation.Amount += (uint)valueToSend;
                    ReputationValueUpdate(reputation, valueToSend);
                }
            }
        }

        public Reputation GetReputation(uint reputationId)
        {
            if (!reputations.TryGetValue(reputationId, out Reputation reputation))
                return ReputationCreate(reputationId);
            return reputation;
        }

        /// <summary>
        /// Used to load the <see cref="CharacterReputation"/> to the player on entering world
        /// </summary>
        /// <returns></returns>
        public List<ServerPlayerCreate.Faction.FactionReputation> LoadReputations()
        {
            List<ServerPlayerCreate.Faction.FactionReputation> factionList = new List<ServerPlayerCreate.Faction.FactionReputation>();

            if (reputations.Count <= 0)
                ReputationCreate((uint)player.Faction1);

            foreach (KeyValuePair<uint, Reputation> reputation in reputations)
                factionList.Add(new ServerPlayerCreate.Faction.FactionReputation
                {
                    FactionId = (ushort)reputation.Value.Id,
                    Value = reputation.Value.Amount
                });

            return factionList;
        }

        public void Save(CharacterContext context)
        {
            foreach (Reputation reputation in reputations.Values)
                reputation.Save(context);
        }

        /// <summary>
        /// Update <see cref="Reputation"/> with supplied amount.
        /// </summary>
        private void ReputationValueUpdate(Reputation reputation, float value)
        {
            if (reputation == null)
                throw new ArgumentNullException();

            player.Session.EnqueueMessageEncrypted(new ServerReputationUpdate
            {
                FactionId = (ushort)reputation.Id,
                Value = value,
            });
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
