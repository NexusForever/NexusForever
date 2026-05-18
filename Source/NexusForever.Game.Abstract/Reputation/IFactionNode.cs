using NexusForever.Game.Static.Reputation;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Reputation
{
    public interface IFactionNode
    {
        Faction FactionId { get; }
        Faction2Entry Entry { get; }
        IFactionNode Parent { get; }

        /// <summary>
        /// Link <see cref="IFactionNode"/> with supplied parent and child <see cref="IFactionNode"/>'s.
        /// </summary>
        void Link(IFactionNode parentNode, List<IFactionNode> childNodes, List<IFactionNode> relationshipNodes);

        /// <summary>
        /// Traverse up tree looking for <see cref="IFactionNode"/> for supplied <see cref="Faction"/>.
        /// </summary>
        IFactionNode GetAscendant(Faction factionId);

        /// <summary>
        /// Traverse down tree looking for <see cref="IFactionNode"/> for supplied <see cref="Faction"/>.
        /// </summary>
        IFactionNode GetDescendent(Faction factionId);

        IEnumerable<IFactionNode> GetChildren();
        IEnumerable<IRelationshipNode> GetRelationships();

        /// <summary>
        /// Return <see cref="FactionLevel"/> 
        /// </summary>
        FactionLevel? GetFriendshipFactionLevel(Faction factionId);
    }
}