using NexusForever.Game.Static.Reputation;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Reputation
{
    public interface IRelationshipNode
    {
        Faction Id { get; }
        Faction2RelationshipEntry Entry { get; }
        IFactionNode Node { get; }

        /// <summary>
        /// Link <see cref="IRelationshipNode"/> with supplied <see cref="IFactionNode"/>.
        /// </summary>
        void Link(IFactionNode node);
    }
}