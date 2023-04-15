using NexusForever.Game.Abstract.Reputation;
using NexusForever.Game.Static.Reputation;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Reputation
{
    public class RelationshipNode : IRelationshipNode
    {
        public Faction Id => (Faction)Entry.FactionId1;
        public Faction2RelationshipEntry Entry { get; }
        public IFactionNode Node { get; private set; }

        /// <summary>
        /// Create a new <see cref="IRelationshipNode"/> with supplied <see cref="Faction2RelationshipEntry"/>.
        /// </summary>
        public RelationshipNode(Faction2RelationshipEntry entry)
        {
            Entry = entry;
        }

        /// <summary>
        /// Link <see cref="IRelationshipNode"/> with supplied <see cref="IFactionNode"/>.
        /// </summary>
        public void Link(IFactionNode node)
        {
            if (Node != null)
                throw new InvalidOperationException();

            Node = node;
        }
    }
}
