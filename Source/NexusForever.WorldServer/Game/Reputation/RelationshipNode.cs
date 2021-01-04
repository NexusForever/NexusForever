using System;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Reputation.Static;

namespace NexusForever.WorldServer.Game.Reputation
{
    public class RelationshipNode
    {
        public Faction Id => (Faction)Entry.FactionId1;
        public Faction2RelationshipEntry Entry { get; }
        public FactionNode Node { get; private set; }

        /// <summary>
        /// Create a new <see cref="RelationshipNode"/> with supplied <see cref="Faction2RelationshipEntry"/>.
        /// </summary>
        public RelationshipNode(Faction2RelationshipEntry entry)
        {
            Entry = entry;
        }

        /// <summary>
        /// Link <see cref="RelationshipNode"/> with supplied <see cref="FactionNode"/>.
        /// </summary>
        public void Link(FactionNode node)
        {
            if (Node != null)
                throw new InvalidOperationException();

            Node = node;
        }
    }
}
