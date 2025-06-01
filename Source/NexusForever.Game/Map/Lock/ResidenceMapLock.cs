using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract;

namespace NexusForever.Game.Map.Lock
{
    public class ResidenceMapLock : MapLock, IResidenceMapLock
    {
        public Identity ResidenceIdentity { get; private set; }

        #region Dependency Injection

        private readonly ILogger<MapLock> log;

        public ResidenceMapLock(
            ILogger<MapLock> log)
            : base(log)
        {
            this.log = log;
        }

        #endregion

        /// <summary>
        /// Initialise residence information for <see cref="IResidenceMapLock"/>.
        /// </summary>
        public void Initialise(Abstract.Identity residenceIdentity)
        {
            if (ResidenceIdentity != null)
                throw new InvalidOperationException();

            ResidenceIdentity = residenceIdentity;
            log.LogTrace($"Set residence id {residenceIdentity.RealmId}:{residenceIdentity.Id} for {InstanceId}");
        }
    }
}
