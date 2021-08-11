using System.Diagnostics;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Housing;
using NexusForever.WorldServer.Game.Static;
using NLog;

namespace NexusForever.WorldServer.Game.Map
{
    public class ResidenceInstancedMap : InstancedMap<ResidenceMapInstance>
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Returns if <see cref="Player"/> can be added to <see cref="ResidenceInstancedMap"/>.
        /// </summary>
        public override GenericError? CanEnter(Player entity, MapPosition position)
        {
            // instance must be specified and exist for residence maps
            if (!position.Info.InstanceId.HasValue
                || GlobalResidenceManager.Instance.GetResidence(position.Info.InstanceId.Value) == null)
                return GenericError.InstanceNotFound;

            return null;
        }

        /// <summary>
        /// Create a new instance for <see cref="Player"/> with <see cref="MapInfo"/>.
        /// </summary>
        protected override ResidenceMapInstance CreateInstance(Player player, MapInfo info)
        {
            var sw = Stopwatch.StartNew();

            Residence residence = null;
            if (info.InstanceId.HasValue)
                // residence already exists but doesn't have an active instance
                residence = GlobalResidenceManager.Instance.GetResidence(info.InstanceId.Value);

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
