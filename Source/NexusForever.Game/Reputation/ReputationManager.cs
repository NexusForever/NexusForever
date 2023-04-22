using System.Collections;
using NexusForever.Database;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Reputation;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Reputation
{
    public class ReputationManager : IReputationManager
    {
        private readonly IPlayer owner;
        private readonly Dictionary<Faction, IReputation> reputations = new();

        /// <summary>
        /// Create a new <see cref="IReputationManager"/> from existing <see cref="CharacterModel"/> database model.
        /// </summary>
        public ReputationManager(IPlayer player, CharacterModel model)
        {
            owner = player;

            foreach (CharacterReputation reputationModel in model.Reputation)
            {
                IFactionNode faction = FactionManager.Instance.GetFaction((Faction)reputationModel.FactionId);
                if (faction == null)
                    throw new DatabaseDataException($"Character {model.Id} has invalid faction {reputationModel.FactionId} stored!");

                var reputation = new Reputation(owner, faction, reputationModel);
                reputations.Add(reputation.Id, reputation);
            }
        }

        public void Save(CharacterContext context)
        {
            foreach (IReputation reputation in reputations.Values)
                reputation.Save(context);
        }

        /// <summary>
        /// Update <see cref="IReputation"/> for supplied <see cref="Faction"/> and value.
        /// </summary>
        /// <remarks>
        /// Value is a delta, if a negative value is supplied it will be deducted from the existing reputation if any.
        /// </remarks>
        public void UpdateReputation(Faction factionId, float value)
        {
            IFactionNode faction = FactionManager.Instance.GetFaction(factionId);
            if (faction == null)
                throw new ArgumentException($"Invalid faction id {factionId}!");

            if (!reputations.TryGetValue(factionId, out IReputation reputation))
            {
                reputation = new Reputation(owner, faction, value);
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
        /// Get <see cref="IReputation"/> for supplied <see cref="Faction"/>.
        /// </summary>
        public IReputation GetReputation(Faction factionId)
        {
            IFactionNode faction = FactionManager.Instance.GetFaction(factionId);
            if (faction == null)
                throw new ArgumentException($"Invalid faction id {factionId}!");

            return reputations.TryGetValue(factionId, out IReputation reputation) ? reputation : null;
        }

        public IEnumerator<IReputation> GetEnumerator()
        {
            return reputations.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
