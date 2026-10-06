using NexusForever.Game.Abstract.Map.Lock;

namespace NexusForever.Game.Map.Lock
{
    public interface IResidenceMapLock : IMapLock
    {
        Abstract.Identity ResidenceIdentity { get; }

        /// <summary>
        /// Initialise residence information for <see cref="IResidenceMapLock"/>.
        /// </summary>
        void Initialise(Abstract.Identity residenceIdentity);
    }
}
