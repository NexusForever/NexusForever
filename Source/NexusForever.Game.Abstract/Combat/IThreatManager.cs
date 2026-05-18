using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Session;

namespace NexusForever.Game.Abstract.Combat
{
    public interface IThreatManager : IEnumerable<IHostileEntity>
    {
        /// <summary>
        /// Returns if any <see cref="IHostileEntity"/> exists in the threat list.
        /// </summary>
        bool IsThreatened { get; }

        /// <summary>
        /// Returns <see cref="IHostileEntity"/> for supplied target if it exists in the threat list.
        /// </summary>
        IHostileEntity GetHostile(uint target);

        /// <summary>
        /// Return the <see cref="IHostileEntity"/> with the highest threat in the threat list.
        /// </summary>
        IHostileEntity GetTopHostile();

        /// <summary>
        /// Add threat for the provided <see cref="IUnitEntity"/>.
        /// </summary>
        void UpdateThreat(IUnitEntity target, int threat);

        /// <summary>
        /// Clear the threat list and alert all entities that this <see cref="IUnitEntity"/> has forgotten them.
        /// </summary>
        void ClearThreatList();

        /// <summary>
        /// Remove the target with the given unit id from this threat list, if they exist. This will alert the entity that they have been forgotten by this <see cref="IUnitEntity"/>.
        /// </summary>
        void RemoveHostile(uint unitId);

        /// <summary>
        /// Broadcast threat list.
        /// </summary>
        /// <remarks>
        /// Threat list will be sent to all <see cref="IPlayer"/>s targeting owner <see cref="IUnitEntity"/>.
        /// </remarks>
        void BroadcastThreatList();

        /// <summary>
        /// Send threat list to supplied <see cref="IGameSession"/>.
        /// </summary>
        void SendThreatList(IGameSession session);
    }
}