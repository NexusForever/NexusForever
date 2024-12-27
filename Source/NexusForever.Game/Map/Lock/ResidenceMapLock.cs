using Microsoft.Extensions.Logging;

namespace NexusForever.Game.Map.Lock
{
    public class ResidenceMapLock : MapLock, IResidenceMapLock
    {
        public ulong ResidenceId { get; private set; }

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
        public void Initialise(ulong residenceId)
        {
            if (ResidenceId != 0)
                throw new InvalidOperationException();

            ResidenceId = residenceId;
            log.LogTrace($"Set residence id {residenceId} for {InstanceId}");
        }
    }
}
