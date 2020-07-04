using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Reputation.Static;

namespace NexusForever.WorldServer.Game.Reputation
{
    public class FactionNode
    {
        /// <summary>
        /// Return faction level based on supplied reputation.
        /// </summary>
        public static FactionLevel GetFactionLevel(float reputation)
        {
            if (reputation >= 32000f)
                return FactionLevel.Beloved;
            if (reputation >= 16000f)
                return FactionLevel.Esteemed;
            if (reputation >= 8000f)
                return FactionLevel.Popular;
            if (reputation >= 4000f)
                return FactionLevel.Accepted;
            if (reputation >= 2000f)
                return FactionLevel.Liked;
            if (reputation >= 0f)
                return FactionLevel.Neutral;
            if (reputation >= -2000f)
                return FactionLevel.Wary;
            if (reputation >= -4000f)
                return FactionLevel.Disliked;
            if (reputation >= -8000f)
                return FactionLevel.Shunned;
            if (reputation >= -16000f)
                return FactionLevel.Hated;

            return FactionLevel.Reviled;
        }

        /// <summary>
        /// Return <see cref="Disposition"/> based on supplied <see cref="FactionLevel"/>.
        /// </summary>
        public static Disposition GetDisposition(FactionLevel factionLevel)
        {
            if (factionLevel >= FactionLevel.Popular)
                return Disposition.Friendly;
            if (factionLevel >= FactionLevel.Disliked)
                return Disposition.Neutral;

            return Disposition.Hostile;
        }

        public Faction FactionId => (Faction)Entry.Id;
        public Faction2Entry Entry { get; }
        public FactionNode Parent { get; private set; }

        private ImmutableList<FactionNode> children;
        private readonly ImmutableList<RelationshipNode> relationships;

        /// <summary>
        /// Create a new <see cref="FactionNode"/> with the supplied <see cref="Faction2Entry"/>.
        /// </summary>
        public FactionNode(Faction2Entry entry)
        {
            Entry = entry;

            // TODO: investigate why some factions have duplicate friendship factions
            relationships = GameTableManager.Instance.Faction2Relationship.Entries
                .Where(e => (Faction)e.FactionId0 == FactionId)
                .GroupBy(e => e.FactionId1)
                .Select(g => new RelationshipNode(g.First()))
                .ToImmutableList();
        }

        /// <summary>
        /// Link <see cref="FactionNode"/> with supplied parent and child <see cref="FactionNode"/>'s.
        /// </summary>
        public void Link(FactionNode parentNode, List<FactionNode> childNodes, List<FactionNode> relationshipNodes)
        {
            if (Parent != null)
                throw new InvalidOperationException();
            if (children != null)
                throw new InvalidOperationException();

            Parent   = parentNode;
            children = childNodes.ToImmutableList();

            foreach (RelationshipNode relationship in relationships)
                relationship.Link(relationshipNodes.SingleOrDefault(n => n.FactionId == relationship.Id));
        }

        /// <summary>
        /// Traverse up tree looking for <see cref="FactionNode"/> for supplied <see cref="Faction"/>.
        /// </summary>
        public FactionNode GetAscendant(Faction factionId)
        {
            if (Parent == null)
                return null;

            if (Parent.FactionId == factionId)
                return Parent;

            return Parent.GetAscendant(factionId);
        }

        /// <summary>
        /// Traverse down tree looking for <see cref="FactionNode"/> for supplied <see cref="Faction"/>.
        /// </summary>
        public FactionNode GetDescendent(Faction factionId)
        {
            foreach (FactionNode child in children)
            {
                if (child.FactionId == factionId)
                    return child;

                FactionNode ss = child.GetDescendent(factionId);
                if (ss != null)
                    return ss;
            }

            return null;
        }

        public IEnumerable<FactionNode> GetChildren()
        {
            return children;
        }

        public IEnumerable<RelationshipNode> GetRelationships()
        {
            return relationships;
        }

        /// <summary>
        /// Return <see cref="FactionLevel"/> 
        /// </summary>
        public FactionLevel? GetFriendshipFactionLevel(Faction factionId)
        {
            foreach (RelationshipNode relationship in relationships)
            {
                if (relationship.Node.FactionId == factionId)
                    return (FactionLevel)relationship.Entry.FactionLevel;

                FactionNode node = relationship.Node.GetDescendent(factionId);
                if (node != null)
                    return (FactionLevel)relationship.Entry.FactionLevel;
            }

            return null;
        }
    }
}
