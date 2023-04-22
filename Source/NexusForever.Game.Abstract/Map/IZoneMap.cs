using NexusForever.Database.Character;
using NexusForever.Game.Abstract.Entity;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Abstract.Map
{
    public interface IZoneMap : IDatabaseCharacter
    {
        /// <summary>
        /// Returns if all <see cref="MapZoneHexGroupEntry"/> in the <see cref="MapZoneEntry"/> have been discovered.
        /// </summary>
        bool IsComplete { get; }

        /// <summary>
        /// Send <see cref="ServerZoneMap"/> to <see cref="IPlayer"/>.
        /// </summary>
        void Send();

        /// <summary>
        /// Add a new discovered <see cref="MapZoneHexGroupEntry"/>.
        /// </summary>
        void AddHexGroup(ushort hexGroupId, bool sendUpdate = true);

        /// <summary>
        /// Returns if the supplied <see cref="MapZoneHexGroupEntry"/> has been discovered.
        /// </summary>
        bool HasHexGroup(ushort hexGroupId);

        /// <summary>
        /// Returns the explored percentage of this <see cref="IZoneMap"/> explored.
        /// </summary>
        /// <remarks>
        /// This is here for testing purposes only. Confirmed that client % matched this % through the course of exploring multiple maps.
        /// </remarks>
        float GetExploredPercent();
    }
}