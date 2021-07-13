using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Map.Static;
using NexusForever.WorldServer.Game.Static;
using NLog;

namespace NexusForever.WorldServer.Game.Map
{
    public abstract class InstancedMap<T> : IMap, IInstancedMap where T : MapInstance, new()
    {
        private class PendingAdd
        {
            public Player Player { get; init; }
            public MapPosition MapPosition { get; init; }
            // see comment further down
            public bool ExplictRemove { get; set; }
        }

        // ReSharper disable once StaticMemberInGenericType
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public WorldEntry Entry { get; private set; }

        private readonly ConcurrentQueue<PendingAdd> pendingAdds = new();
        private readonly Dictionary</*instanceId*/ ulong, T> instances = new();

        /// <summary>
        /// Initialise <see cref="InstancedMap{T}"/> with <see cref="WorldEntry"/>.
        /// </summary>
        public void Initialise(WorldEntry entry)
        {
            Entry = entry;
        }

        /// <summary>
        /// Enqueue <see cref="GridEntity"/> to be added to <see cref="InstancedMap{T}"/>.
        /// </summary>
        /// <remarks>
        /// Characters should not be added directly through this method.
        /// Use <see cref="Player.TeleportTo(MapPosition, TeleportReason)"/> instead.
        /// </remarks>
        public void EnqueueAdd(GridEntity entity, MapPosition position)
        {
            // entities can't be added directly to an instance map
            // they need to be added to the map instance instead
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Returns if <see cref="GridEntity"/> can be added to <see cref="InstancedMap{T}"/>.
        /// </summary>
        public virtual bool CanEnter(GridEntity entity, MapPosition position)
        {
            return false;
        }

        /// <summary>
        /// Enqueue <see cref="Player"/> to be added to <see cref="IMap"/>.
        /// </summary>
        /// <remarks>
        /// Characters should not be added directly through this method.
        /// Use <see cref="Player.TeleportTo(MapPosition, TeleportReason)"/> instead.
        /// </remarks>
        public void EnqueueAdd(Player player, MapPosition position)
        {
            pendingAdds.Enqueue(new PendingAdd
            {
                Player      = player,
                MapPosition = position
            });
        }

        /// <summary>
        /// Returns if <see cref="Player"/> can be added to <see cref="InstancedMap{T}"/>.
        /// </summary>
        public virtual GenericError? CanEnter(Player player, MapPosition position)
        {
            if (!MapManager.Instance.CanCreateInstance(player))
                return GenericError.InstanceLimitExceeded;

            return null;
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            ProcessPending();
            UpdateInstances(lastTick);
        }

        private void ProcessPending()
        {
            var newActions = new List<PendingAdd>();
            while (pendingAdds.TryDequeue(out PendingAdd pending))
            {
                T instance = null;
                if (pending.MapPosition.Info.InstanceId.HasValue)
                    instance = GetInstance(pending.MapPosition.Info.InstanceId.Value);

                // specified instance doesn't have an instance or no specific instance was specified
                if (instance == null)
                {
                    instance = CreateInstance(pending.Player, pending.MapPosition.Info);
                    instances.Add(instance.InstanceId, instance);

                    log.Trace($"Created new instance {instance.InstanceId} for map {instance.Entry.Id}.");

                    MapManager.Instance.IncreaseInstanceCount(pending.Player);
                }

                // existing instance is unloading
                if (instance.UnloadStatus.HasValue)
                {
                    // this is a special rare case when a player is removed from an instance during unload but the return position is the same instance
                    // in this case we must explicitly remove the player from the instance and wait for the instance to unload before recreating
                    // you can replicate this functionality by invoking the instance unload command while on your own residence
                    if (instance.UnloadStatus == MapUnloadStatus.UnloadingPlayers
                        && pending.Player.Map == instance
                        && !pending.ExplictRemove)
                    {
                        pending.Player.RemoveFromMap();
                        pending.ExplictRemove = true;
                    }

                    // wait for instance to completely unload before recreating and adding
                    newActions.Add(pending);
                    continue;
                }

                GenericError? result = instance.CanEnter(pending.Player, pending.MapPosition);
                if (result.HasValue)
                {
                    pending.Player.OnTeleportToFailed(result.Value);
                    return;
                }

                instance.EnqueueAdd(pending.Player, pending.MapPosition);
            }

            // new actions are added to the queue after processing so they are processed starting next update
            foreach (PendingAdd action in newActions)
                pendingAdds.Enqueue(action);
        }

        private void UpdateInstances(double lastTick)
        {
            var unloadedInstances = new List<MapInstance>();
            foreach (T map in instances.Values)
            {
                map.Update(lastTick);

                if (map.UnloadStatus == MapUnloadStatus.Complete)
                    unloadedInstances.Add(map);
            }

            foreach (MapInstance instance in unloadedInstances)
            {
                instances.Remove(instance.InstanceId);
                log.Trace($"Unloaded existing instance {instance.InstanceId} for map {instance.Entry.Id}.");
            }
        }

        /// <summary>
        /// Create a new instance for <see cref="Player"/> with <see cref="MapInfo"/>.
        /// </summary>
        protected abstract T CreateInstance(Player player, MapInfo info);

        /// <summary>
        /// Get an existing instance with supplied id.
        /// </summary>
        protected virtual T GetInstance(ulong instanceId)
        {
            return instances.TryGetValue(instanceId, out T map) ? map : default;
        }
    }
}
