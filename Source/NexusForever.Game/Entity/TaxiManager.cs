using System.Numerics;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Reputation;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared.Game;

namespace NexusForever.Game.Entity
{
    public class TaxiManager : ITaxiManager
    {
        /// <summary>
        /// Distance in units at which a taxi kiosk is discovered by walking past it.
        /// </summary>
        private const float DiscoveryRange = 30f;

        private readonly IPlayer player;
        private readonly HashSet<ushort> taxiNodes = [];
        private readonly HashSet<ushort> unsavedTaxiNodes = [];

        private UpdateTimer discoveryTimer = new(TimeSpan.FromSeconds(2));

        /// <summary>
        /// Create a new <see cref="ITaxiManager"/> from existing <see cref="CharacterModel"/> database model.
        /// </summary>
        public TaxiManager(IPlayer owner, CharacterModel model)
        {
            player = owner;

            foreach (CharacterTaxiNodeModel taxiNodeModel in model.TaxiNode)
                taxiNodes.Add(taxiNodeModel.TaxiNodeId);
        }

        public void Update(double lastTick)
        {
            discoveryTimer.Update(lastTick);
            if (discoveryTimer.HasElapsed)
            {
                DiscoverNearbyTaxiNodes();
                discoveryTimer.Reset();
            }
        }

        public void Save(CharacterContext context)
        {
            foreach (ushort taxiNodeId in unsavedTaxiNodes)
            {
                context.Add(new CharacterTaxiNodeModel
                {
                    Id         = player.CharacterId,
                    TaxiNodeId = taxiNodeId
                });
            }

            unsavedTaxiNodes.Clear();
        }

        /// <summary>
        /// Send the discovered taxi nodes to the client.
        /// </summary>
        public void SendInitialPackets()
        {
            player.Session.EnqueueMessageEncrypted(new ServerFlightPathUpdate
            {
                TaxiNodeIds = taxiNodes.Select(t => (uint)t).ToList()
            });
        }

        /// <summary>
        /// Discover the supplied taxi node, the taxi map offers routes through it from now on.
        /// </summary>
        public void DiscoverTaxiNode(uint taxiNodeId)
        {
            TaxiNodeEntry entry = GameTableManager.Instance.TaxiNode.GetEntry(taxiNodeId);
            if (entry == null)
                return;

            // TaxiNode.taxiNodeFactionEnum: 0 neutral, 1 Exile, 2 Dominion
            uint faction = player.Faction1 switch
            {
                Faction.Exile    => 1u,
                Faction.Dominion => 2u,
                _                => 0u
            };
            if (entry.TaxiNodeFactionEnum != 0u && faction != 0u && entry.TaxiNodeFactionEnum != faction)
                return;

            if (!taxiNodes.Add((ushort)taxiNodeId))
                return;

            unsavedTaxiNodes.Add((ushort)taxiNodeId);
            player.Session.EnqueueMessageEncrypted(new ServerTaxiNodeUnlock
            {
                TaxiNodeId = (ushort)taxiNodeId
            });
        }

        /// <summary>
        /// Return true if the supplied taxi node has been discovered.
        /// </summary>
        public bool IsTaxiNodeKnown(uint taxiNodeId)
        {
            return taxiNodes.Contains((ushort)taxiNodeId);
        }

        /// <summary>
        /// Return true if a kiosk of the supplied taxi node is visible and within range of the owner.
        /// </summary>
        public bool IsNearTaxiNode(uint taxiNodeId, float range)
        {
            return player.GetVisible()
                .OfType<IWorldEntity>()
                .Any(e => e.CreatureEntry?.TaxiNodeId == taxiNodeId
                    && Vector3.DistanceSquared(e.Position, player.Position) <= range * range);
        }

        /// <summary>
        /// Discover the taxi kiosks (Creature2.taxiNodeId) the owner is standing near.
        /// </summary>
        private void DiscoverNearbyTaxiNodes()
        {
            foreach (IWorldEntity kiosk in player.GetVisible().OfType<IWorldEntity>().ToList())
            {
                uint taxiNodeId = kiosk.CreatureEntry?.TaxiNodeId ?? 0u;
                if (taxiNodeId == 0u || IsTaxiNodeKnown(taxiNodeId))
                    continue;

                if (Vector3.DistanceSquared(kiosk.Position, player.Position) <= DiscoveryRange * DiscoveryRange)
                    DiscoverTaxiNode(taxiNodeId);
            }
        }
    }
}
