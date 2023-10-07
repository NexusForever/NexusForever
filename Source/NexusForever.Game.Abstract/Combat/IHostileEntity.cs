using NexusForever.Network.Message;

namespace NexusForever.Game.Abstract.Combat
{
    public interface IHostileEntity
    {
        uint HatedUnitId { get; }
        uint Threat { get; }

        /// <summary>
        /// Modify this <see cref="IHostileEntity"/> threat by the given amount.
        /// </summary>
        /// <remarks>
        /// Value is a delta, if a negative value is supplied it will be deducted from the existing threat if any.
        /// </remarks>
        void UpdateThreat(int threatDelta);
    }
}