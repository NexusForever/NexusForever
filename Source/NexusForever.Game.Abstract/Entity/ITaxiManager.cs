using NexusForever.Database.Character;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Entity
{
    public interface ITaxiManager : IDatabaseCharacter, IUpdate
    {
        /// <summary>
        /// Discover the supplied taxi node, the taxi map offers routes through it from now on.
        /// </summary>
        void DiscoverTaxiNode(uint taxiNodeId);

        /// <summary>
        /// Return true if the supplied taxi node has been discovered.
        /// </summary>
        bool IsTaxiNodeKnown(uint taxiNodeId);

        /// <summary>
        /// Return true if a kiosk of the supplied taxi node is visible and within range of the owner.
        /// </summary>
        bool IsNearTaxiNode(uint taxiNodeId, float range);

        /// <summary>
        /// Send the discovered taxi nodes to the client.
        /// </summary>
        void SendInitialPackets();
    }
}
