using NexusForever.Game.Abstract.Combat;
using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Game.Combat
{
    public class HostileEntity : IHostileEntity
    {
        public IUnitEntity Owner { get; private set; }
        public uint HatedUnitId { get; private set; }
        public uint Threat { get; private set; }

        /// <summary>
        /// Create a new <see cref="IHostileEntity"/> for the given <see cref="IUnitEntity"/>.
        /// </summary>
        public HostileEntity(IUnitEntity hater, IUnitEntity target)
        {
            Owner = hater;
            HatedUnitId = target.Guid;
        }

        /// <summary>
        /// Modify this <see cref="IHostileEntity"/> threat by the given amount.
        /// </summary>
        /// <remarks>
        /// Value is a delta, if a negative value is supplied it will be deducted from the existing threat if any.
        /// </remarks>
        public void AdjustThreat(int threatDelta)
        {
            Threat = (uint)Math.Clamp(Threat + threatDelta, 0u, uint.MaxValue);
        }

        /// <summary>
        /// Returns the <see cref="IUnitEntity"/> that this <see cref="IHostileEntity"/> is associated with.
        /// </summary>
        public IUnitEntity GetEntity(IWorldEntity requester)
        {
            return requester?.Map?.GetEntity<IUnitEntity>(HatedUnitId);
        }
    }
}
