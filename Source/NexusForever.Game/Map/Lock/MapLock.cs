using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Map.Lock;
using NexusForever.Game.Static.Map.Lock;

namespace NexusForever.Game.Map.Lock
{
    public class MapLock : IMapLock
    {
        public Guid InstanceId { get; private set; }
        public MapLockType Type { get; private set; }
        public uint WorldId { get; private set; }

        private readonly HashSet<Identity> characters = [];

        #region Dependency Injection

        private readonly ILogger<MapLock> log;

        public MapLock(
            ILogger<MapLock> log)
        {
            this.log = log;
        }

        #endregion

        /// <summary>
        /// Initialise new map lock with supplied <see cref="MapLockType"/> and world id.
        /// </summary>
        public void Initialise(MapLockType mapLockType, uint worldId)
        {
            if (InstanceId != Guid.Empty)
                throw new InvalidOperationException();

            InstanceId = Guid.NewGuid();
            Type       = mapLockType;
            WorldId    = worldId;

            log.LogTrace($"Initialised new {Type} map lock for world {WorldId} with instance id {InstanceId}");
        }

        /// <summary>
        /// Add a character to the map lock.
        /// </summary>
        public void AddCharacer(Identity identity)
        {
            characters.Add(identity);
            log.LogTrace($"Added character {identity} to map lock {InstanceId}");
        }

        /// <summary>
        /// Remove a character from the map lock.
        /// </summary>
        public void RemoveCharacter(Identity identity)
        {
            characters.Remove(identity);
            log.LogTrace($"Removed character {identity} from map lock {InstanceId}");
        }
    }
}
