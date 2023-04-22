using System.Diagnostics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Housing;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Housing;
using NexusForever.Network.World.Message.Static;
using NLog;

namespace NexusForever.Game.Map
{
    public class ResidenceInstancedMap : InstancedMap<IResidenceMapInstance>
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can be added to <see cref="ResidenceInstancedMap"/>.
        /// </summary>
        public override GenericError? CanEnter(IPlayer entity, IMapPosition position)
        {
            // instance must be specified and exist for residence maps
            if (!position.Info.InstanceId.HasValue
                || GlobalResidenceManager.Instance.GetResidence(position.Info.InstanceId.Value) == null)
                return GenericError.InstanceNotFound;

            return null;
        }

        /// <summary>
        /// Create a new instance for <see cref="IPlayer"/> with <see cref="IMapInfo"/>.
        /// </summary>
        public override IResidenceMapInstance CreateInstance(IPlayer player, ulong? instanceId)
        {
            var sw = Stopwatch.StartNew();

            IResidence residence = null;
            if (instanceId.HasValue)
                // residence already exists but doesn't have an active instance
                residence = GlobalResidenceManager.Instance.GetResidence(instanceId.Value);

            // this shouldn't occur as a residence should always be created before adding a player to a residence map
            // here just in case that doesn't occur
            residence ??= GlobalResidenceManager.Instance.CreateResidence(player);

            var instance = new ResidenceMapInstance
            {
                InstanceId = residence.Id
            };
            instance.Initialise(Entry);
            instance.Initialise(residence);

            sw.Stop();
            if (sw.ElapsedMilliseconds > 10)
                log.Warn($"Took {sw.ElapsedMilliseconds}ms to create instance for residence {residence.Id}!");

            return instance;
        }
    }
}
