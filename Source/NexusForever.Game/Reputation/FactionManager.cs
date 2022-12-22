using System.Collections.Immutable;
using NexusForever.Game.Static.Reputation;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Shared;
using NLog;

namespace NexusForever.Game.Reputation
{
    public class FactionManager : Singleton<FactionManager>
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private ImmutableDictionary<Faction, FactionNode> nodes;

        private FactionManager()
        {
        }

        public void Initialise()
        {
            DateTime start = DateTime.Now;
            log.Info("Initialising factions...");

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
            log.Info($"Initialised {nodes.Count} faction(s) in {span.TotalMilliseconds}ms.");
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
