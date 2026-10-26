using System.Linq;
using System.Numerics;
using Microsoft.Extensions.Logging;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Entity.Player
{
    /// <summary>
    /// Taxi kiosk flights and shuttles, the player pays for the route and is moved to the destination of its last leg.
    /// </summary>
    /// <remarks>
    /// The price is the sum of TaxiRoute.price for each leg, the same price the taxi map shows.
    /// Retail flew the player along the route splines, for now the player is teleported to the destination.
    /// </remarks>
    public class ClientTaxiFlightHandler : IMessageHandler<IWorldSession, ClientTaxiFlight>
    {
        private const float KioskRange = 15f;
        private const int MaxRouteLength = 30;

        #region Dependency Injection

        private readonly ILogger<ClientTaxiFlightHandler> log;
        private readonly IGameTableManager gameTableManager;

        public ClientTaxiFlightHandler(
            ILogger<ClientTaxiFlightHandler> log,
            IGameTableManager gameTableManager)
        {
            this.log              = log;
            this.gameTableManager = gameTableManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientTaxiFlight taxiFlight)
        {
            IPlayer player = session.Player;
            if (taxiFlight.Route.Count == 0 || taxiFlight.Route.Count > MaxRouteLength)
                return;

            TaxiRouteEntry[] legs = GetLegs(taxiFlight.Route.ToArray());
            if (legs == null)
            {
                log.LogWarning($"Taxi request from {player.Name} refused, route {string.Join(",", taxiFlight.Route)} is not a chain of valid legs.");
                return;
            }

            if (!CanFly(player, legs))
                return;

            uint destination = legs[^1].TaxiNodeIdDestination;
            TaxiNodeEntry destinationEntry = gameTableManager.TaxiNode.GetEntry(destination);
            WorldLocation2Entry location = gameTableManager.WorldLocation2.GetEntry(destinationEntry.WorldLocation2Id);
            if (location == null || gameTableManager.World.GetEntry(location.WorldId) == null)
            {
                log.LogWarning($"Taxi request from {player.Name} refused, node {destination} has no valid world location.");
                return;
            }

            if (!player.CanTeleport())
                return;

            ulong price = legs.Aggregate(0ul, (sum, leg) => sum + leg.Price);
            if (!player.CurrencyManager.CanAfford(CurrencyType.Credits, price))
                return;

            player.CurrencyManager.CurrencySubtractAmount(CurrencyType.Credits, price);

            // flying somewhere discovers it
            player.DiscoverTaxiNode(destination);

            var rotation = new Quaternion(location.Facing0, location.Facing1, location.Facing2, location.Facing3);
            player.Rotation = rotation.ToEuler();
            player.TeleportTo((ushort)location.WorldId, location.Position0, location.Position1, location.Position2);
        }

        /// <summary>
        /// Return the legs of the route, null if a leg or node is invalid or a leg doesn't start where the previous one ended.
        /// </summary>
        private TaxiRouteEntry[] GetLegs(uint[] routeIds)
        {
            var legs = new TaxiRouteEntry[routeIds.Length];
            for (int i = 0; i < routeIds.Length; i++)
            {
                TaxiRouteEntry leg = gameTableManager.TaxiRoute.GetEntry(routeIds[i]);
                if (leg == null
                    || gameTableManager.TaxiNode.GetEntry(leg.TaxiNodeIdSource) == null
                    || gameTableManager.TaxiNode.GetEntry(leg.TaxiNodeIdDestination) == null)
                    return null;

                if (i > 0 && legs[i - 1].TaxiNodeIdDestination != leg.TaxiNodeIdSource)
                    return null;

                legs[i] = leg;
            }

            return legs;
        }

        /// <summary>
        /// The player has to be at a kiosk of the first node and every node on the route has to be neutral or of their faction.
        /// </summary>
        private bool CanFly(IPlayer player, TaxiRouteEntry[] legs)
        {
            // TaxiNode.taxiNodeFactionEnum: 0 neutral, 1 Exile, 2 Dominion
            uint faction = player.Faction1 switch
            {
                Faction.Exile    => 1u,
                Faction.Dominion => 2u,
                _                => 0u
            };

            bool otherFaction = legs
                .SelectMany(leg => new[] { leg.TaxiNodeIdSource, leg.TaxiNodeIdDestination })
                .Select(id => gameTableManager.TaxiNode.GetEntry(id).TaxiNodeFactionEnum)
                .Any(f => f != 0u && faction != 0u && f != faction);
            if (otherFaction)
                return false;

            uint source = legs[0].TaxiNodeIdSource;
            if (!player.IsNearTaxiNode(source, KioskRange))
            {
                log.LogWarning($"Taxi request from {player.Name} refused, no kiosk of node {source} within range.");
                return false;
            }

            return true;
        }
    }
}
