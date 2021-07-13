using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Housing;
using NexusForever.WorldServer.Game.Static;

namespace NexusForever.WorldServer.Game.Map
{
    public class ResidenceInstancedMap : InstancedMap<ResidenceMapInstance>
    {
        /// <summary>
        /// Returns if <see cref="Player"/> can be added to <see cref="ResidenceInstancedMap"/>.
        /// </summary>
        public override GenericError? CanEnter(Player entity, MapPosition position)
        {
            // instance must be specified and exist for residence maps
            if (!position.Info.InstanceId.HasValue
                || ResidenceManager.Instance.GetResidence(position.Info.InstanceId.Value)
                .GetAwaiter()
                .GetResult() == null)
                return GenericError.InstanceNotFound;

            return null;
        }

        /// <summary>
        /// Create a new instance for <see cref="Player"/> with <see cref="MapInfo"/>.
        /// </summary>
        protected override ResidenceMapInstance CreateInstance(Player player, MapInfo info)
        {
            Residence residence = null;
            if (info.InstanceId.HasValue)
            {
                // residence already exists but doesn't have an active instance
                residence = ResidenceManager.Instance.GetResidence(info.InstanceId.Value)
                    .GetAwaiter()
                    .GetResult();
            }

            // this shouldn't occur as a residence should always be created before adding a player to a residence map
            // here just in case that doesn't occur
            residence ??= ResidenceManager.Instance.CreateResidence(player);

            var instance = new ResidenceMapInstance
            {
                InstanceId = residence.Id
            };
            instance.Initialise(Entry);
            instance.Initialise(residence);

            return instance;
        }
    }
}
