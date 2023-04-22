using NexusForever.Game.Static.Reputation;

namespace NexusForever.Game.Abstract.Reputation
{
    public interface IFactionManager
    {
        void Initialise();

        /// <summary>
        /// Return the <see cref="IFactionNode"/> with the supplied faction id.
        /// </summary>
        IFactionNode GetFaction(Faction factionId);
    }
}