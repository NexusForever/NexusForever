using NexusForever.Shared;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Reputation.Static;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NexusForever.WorldServer.Game.Reputation
{
    public class FactionManager : AbstractManager<FactionManager>
    {
        private ImmutableDictionary<Faction, FactionNode> nodes;

        private FactionManager()
        {
        }

        public override FactionManager Initialise()
        {
            DateTime start = DateTime.Now;
            Log.Info("Initialising factions...");

            var builder = ImmutableDictionary.CreateBuilder<Faction, FactionNode>();
            foreach (Faction2Entry entry in GameTableManager.Instance.Faction2.Entries)
                builder.Add((Faction)entry.Id, new FactionNode(entry));

            // link nodes and build the trees
            foreach (FactionNode node in builder.Values)
            {
                // parent
                builder.TryGetValue((Faction)node.Entry.Faction2IdParent, out FactionNode parent);

                // children
                var children = new List<FactionNode>();
                foreach (Faction2Entry entry in GameTableManager.Instance.Faction2.Entries
                    .Where(e => (Faction)e.Faction2IdParent == node.FactionId))
                {
                    builder.TryGetValue((Faction)entry.Id, out FactionNode child);
                    children.Add(child);
                }

                // relationships
                var relationships = new List<FactionNode>();
                foreach (RelationshipNode relationship in node.GetRelationships())
                {
                    builder.TryGetValue((Faction)relationship.Entry.FactionId1, out FactionNode child);
                    relationships.Add(child);
                }

                node.Link(parent, children, relationships);
            }

            nodes = builder.ToImmutable();

            TimeSpan span = DateTime.Now - start;
            Log.Info($"Initialised {nodes.Count} faction(s) in {span.TotalMilliseconds}ms.");
            return Instance;
        }

        /// <summary>
        /// Return the <see cref="FactionNode"/> with the supplied faction id.
        /// </summary>
        public FactionNode GetFaction(Faction factionId)
        {
            return nodes.TryGetValue(factionId, out FactionNode node) ? node : null;
        }
    }
}
