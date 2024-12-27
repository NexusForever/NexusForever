using NexusForever.Game.Abstract.Map.Lock;

namespace NexusForever.Game.Map.Lock
{
    public interface IResidenceMapLock : IMapLock
    {
        ulong ResidenceId { get; }

        /// <summary>
        /// Initialise residence information for <see cref="IResidenceMapLock"/>.
        /// </summary>
        void Initialise(ulong residenceId);
    }
}
