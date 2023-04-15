using NexusForever.Database.Character;
using NexusForever.Game.Static.Reputation;

namespace NexusForever.Game.Abstract.Reputation
{
    public interface IReputationManager : IDatabaseCharacter, IEnumerable<IReputation>
    {
        /// <summary>
        /// Update <see cref="IReputation"/> for supplied <see cref="Faction"/> and value.
        /// </summary>
        /// <remarks>
        /// Value is a delta, if a negative value is supplied it will be deducted from the existing reputation if any.
        /// </remarks>
        void UpdateReputation(Faction factionId, float value);

        /// <summary>
        /// Get <see cref="IReputation"/> for supplied <see cref="Faction"/>.
        /// </summary>
        IReputation GetReputation(Faction factionId);
    }
}