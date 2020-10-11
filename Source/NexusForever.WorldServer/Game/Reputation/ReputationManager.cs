using System;
using System.Collections;
using System.Collections.Generic;
using NexusForever.Database;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Reputation.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Reputation
{
    public class ReputationManager : ISaveCharacter, IEnumerable<Reputation>
    {
        private readonly Player owner;
        private readonly Dictionary<Faction, Reputation> reputations = new Dictionary<Faction, Reputation>();

        /// <summary>
        /// Create a new <see cref="ReputationManager"/> from existing <see cref="CharacterModel"/> database model.
        /// </summary>
        public ReputationManager(Player player, CharacterModel model)
        {
            owner = player;

            foreach (CharacterReputation reputationModel in model.Reputation)
            {
                FactionNode faction = FactionManager.Instance.GetFaction((Faction)reputationModel.FactionId);
                if (faction == null)
                    throw new DatabaseDataException($"Character {model.Id} has invalid faction {reputationModel.FactionId} stored!");

                var reputation = new Reputation(faction, reputationModel);
                reputations.Add(reputation.Id, reputation);
            }
        }

        public void Save(CharacterContext context)
        {
            foreach (Reputation reputation in reputations.Values)
                reputation.Save(owner.CharacterId, context);
        }

        /// <summary>
        /// Update <see cref="Reputation"/> for supplied <see cref="Faction"/> and value.
        /// </summary>
        /// <remarks>
        /// Value is a delta, if a negative value is supplied it will be deducted from the existing reputation if any.
        /// </remarks>
        public void UpdateReputation(Faction factionId, float value)
        {
            FactionNode faction = FactionManager.Instance.GetFaction(factionId);
            if (faction == null)
                throw new ArgumentException($"Invalid faction id {factionId}!");

            if (!reputations.TryGetValue(factionId, out Reputation reputation))
            {
                reputation = new Reputation(faction, value);
                reputations.Add(reputation.Id, reputation);
            }
            else
                reputation.Amount += value;

            owner.Session.EnqueueMessageEncrypted(new ServerReputationUpdate
            {
                FactionId = factionId,
                Value     = value
            });
        }

        /// <summary>
        /// Get <see cref="Reputation"/> for supplied <see cref="Faction"/>.
        /// </summary>
        public Reputation GetReputation(Faction factionId)
        {
            FactionNode faction = FactionManager.Instance.GetFaction(factionId);
            if (faction == null)
                throw new ArgumentException($"Invalid faction id {factionId}!");

            return reputations.TryGetValue(factionId, out Reputation reputation) ? reputation : null;
        }

        public IEnumerator<Reputation> GetEnumerator()
        {
            return reputations.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
