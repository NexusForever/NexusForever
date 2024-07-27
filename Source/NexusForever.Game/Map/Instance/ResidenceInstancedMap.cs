using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Housing;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Abstract.Map.Lock;
using NexusForever.Game.Map.Lock;
using NexusForever.Network.World.Message.Static;
using NexusForever.Shared;

namespace NexusForever.Game.Map.Instance
{
    public class ResidenceInstancedMap : InstancedMap<IResidenceMapInstance>
    {
        #region Dependency Injection

        private readonly ILogger<ResidenceInstancedMap> log;

        private readonly IMapLockManager mapLockManager;
        private readonly IFactory<IResidenceMapInstance> instanceFactory;
        private readonly IGlobalResidenceManager globalResidenceManager;

        public ResidenceInstancedMap(
            ILogger<ResidenceInstancedMap> log,
            IMapLockManager mapLockManager,
            IFactory<IResidenceMapInstance> instanceFactory,
            IGlobalResidenceManager globalResidenceManager)
        {
            this.log = log;

            this.mapLockManager = mapLockManager;
            this.instanceFactory = instanceFactory;
            this.globalResidenceManager = globalResidenceManager;
        }

        #endregion

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can be added to <see cref="ResidenceInstancedMap"/>.
        /// </summary>
        public override GenericError? CanEnter(IPlayer entity, IMapPosition position)
        {
            if (position.Info.MapLock is IResidenceMapLock residenceMapLock
                && globalResidenceManager.GetResidence(residenceMapLock.ResidenceId) == null)
                return GenericError.InstanceNotFound;

            return null;
        }

        protected override IMapLock GetMapLock(IPlayer player)
        {
            IResidence residence = player.ResidenceManager.Residence;
            // residence should always exist, here just incase
            residence ??= globalResidenceManager.CreateResidence(player);

            return mapLockManager.GetResidenceLock(residence);
        }

        protected override void UpdatePosition(IPlayer player, IMapPosition mapPosition)
        {
            if (mapPosition.Info.MapLock != null)
                return;

            // if no map lock is specified, player is entering own residence, return to entrance
            IResidence residence = player.ResidenceManager.Residence;

            IResidenceEntrance entrance = globalResidenceManager.GetResidenceEntrance(residence.PropertyInfoId);
            mapPosition.Position = entrance.Position;
            player.Rotation = entrance.Rotation.ToEulerDegrees();
        }

        protected override IResidenceMapInstance CreateInstance(IPlayer player, IMapLock mapLock)
        {
            IResidence residence = globalResidenceManager.GetResidence((mapLock as IResidenceMapLock).ResidenceId);

            IResidenceMapInstance residenceMapInstance = instanceFactory.Resolve();
            residenceMapInstance.Initialise(Entry, mapLock);
            residenceMapInstance.Initialise(residence);

            return residenceMapInstance;
        }
    }
}
