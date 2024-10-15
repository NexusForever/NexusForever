using System.Collections.Concurrent;
using System.Text;
using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Abstract.Map.Lock;
using NexusForever.Game.Configuration.Model;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Map;
using NexusForever.Game.Static.RBAC;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Static;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Game;

namespace NexusForever.Game.Map.Instance
{
    public abstract class InstancedMap<T> : IMap, IInstancedMap where T : IMapInstance
    {
        private class PendingAdd
        {
            public IPlayer Player { get; init; }
            public IMapPosition MapPosition { get; init; }
            public OnAddDelegate Callback { get; init; }
            public OnGenericErrorDelegate Error { get; init; }
            public OnExceptionDelegate Exception { get; init; }

            // see comment further down
            public bool ExplictRemove { get; set; }
        }

        public WorldEntry Entry { get; private set; }

        private readonly ConcurrentQueue<PendingAdd> pendingAdds = new();
        private readonly Dictionary</*instanceId*/ Guid, T> instances = new();

        private readonly Dictionary<ulong, uint> instanceCounts = new();
        // reset instance limit counts every hour
        private readonly UpdateTimer instanceCountReset = new(TimeSpan.FromMinutes(60));

        #region Dependency Injection

        private readonly ILogger log;

        public InstancedMap(
            ILogger log)
        {
            this.log = log;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IInstancedMap{T}"/> with <see cref="WorldEntry"/>.
        /// </summary>
        public void Initialise(WorldEntry entry)
        {
            Entry = entry;
        }

        /// <summary>
        /// Enqueue <see cref="IPlayer"/> to be added to <see cref="IInstancedMap{T}"/>.
        /// </summary>
        /// <remarks>
        /// Characters should not be added directly through this method.
        /// Use <see cref="IPlayer.TeleportTo(IMapPosition,TeleportReason)"/> instead.
        /// </remarks>
        public void EnqueueAdd(IPlayer player, IMapPosition position, OnAddDelegate callback = null, OnGenericErrorDelegate error = null, OnExceptionDelegate exception = null)
        {
            pendingAdds.Enqueue(new PendingAdd
            {
                Player      = player,
                MapPosition = position,
                Callback    = callback,
                Error       = error,
                Exception   = exception
            });
        }

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can be added to <see cref="IInstancedMap{T}"/>.
        /// </summary>
        protected virtual GenericError? CanEnter(IPlayer player, IMapPosition position)
        {
            if (!CanCreateInstance(player))
                return GenericError.InstanceLimitExceeded;

            return null;
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            instanceCountReset.Update(lastTick);
            if (instanceCountReset.HasElapsed)
            {
                instanceCounts.Clear();
                instanceCountReset.Reset();
            }

            ProcessPending();
            UpdateInstances(lastTick);
        }

        private void ProcessPending()
        {
            var newActions = new List<PendingAdd>();
            while (pendingAdds.TryDequeue(out PendingAdd pending))
            {
                try
                {
                    if (!ProcessPendingAdd(pending))
                        newActions.Add(pending);
                }
                catch (Exception ex)
                {
                    pending.Exception?.Invoke(ex);
                }
            }

            // new actions are added to the queue after processing so they are processed starting next update
            foreach (PendingAdd action in newActions)
                pendingAdds.Enqueue(action);
        }

        private bool ProcessPendingAdd(PendingAdd pending)
        {
            GenericError? error = CanEnter(pending.Player, pending.MapPosition);
            if (error.HasValue)
            {
                pending.Error?.Invoke(error.Value);
                return true;
            }

            IMapLock mapLock = pending.MapPosition.Info.MapLock;

            // find implicit map lock if explicit not specified
            mapLock ??= GetMapLock(pending.Player);
            if (mapLock == null)
                throw new InvalidOperationException();

            T instance = GetInstance(mapLock.InstanceId);

            // specified instance doesn't have an instance yet, create it
            if (instance == null)
            {
                instance = CreateInstance(pending.Player, mapLock);
                instances.Add(instance.InstanceId, instance);

                log.LogTrace($"Created new instance {instance.InstanceId} for map {instance.Entry.Id}.");

                IncreaseInstanceCount(pending.Player);
            }

            // existing instance is unloading
            if (instance.UnloadStatus.HasValue)
            {
                // this is a special rare case when a player is removed from an instance during unload but the return position is the same instance
                // in this case we must explicitly remove the player from the instance and wait for the instance to unload before recreating
                // you can replicate this functionality by invoking the instance unload command while on your own residence
                if (instance.UnloadStatus == MapUnloadStatus.UnloadingPlayers
                    && pending.Player.Map == (IBaseMap)instance
                    && !pending.ExplictRemove)
                {
                    pending.Player.RemoveFromMap();
                    pending.ExplictRemove = true;
                }

                // wait for instance to completely unload before recreating and adding
                return false;
            }

            UpdatePosition(pending.Player, pending.MapPosition);
            instance.EnqueueAdd(pending.Player, pending.MapPosition.Position, pending.Callback, pending.Error, pending.Exception);

            return true;
        }

        private void UpdateInstances(double lastTick)
        {
            var unloadedInstances = new List<IMapInstance>();
            foreach (T map in instances.Values)
            {
                map.Update(lastTick);

                if (map.UnloadStatus == MapUnloadStatus.Complete)
                    unloadedInstances.Add(map);
            }

            foreach (IMapInstance instance in unloadedInstances)
            {
                instances.Remove(instance.InstanceId);
                log.LogTrace($"Unloaded existing instance {instance.InstanceId} for map {instance.Entry.Id}.");
            }
        }

        protected abstract IMapLock GetMapLock(IPlayer player);

        protected abstract T CreateInstance(IPlayer player, IMapLock mapLock);

        protected virtual void UpdatePosition(IPlayer player, IMapPosition mapPosition)
        {
            // deliberately empty
        }

        /// <summary>
        /// Get an existing instance with supplied id.
        /// </summary>
        public virtual T GetInstance(Guid instanceId)
        {
            return instances.TryGetValue(instanceId, out T map) ? map : default;
        }

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can create a new instance.
        /// </summary>
        private bool CanCreateInstance(IPlayer player)
        {
            if (instanceCounts.TryGetValue(player.CharacterId, out uint instanceCount)
                && instanceCount > (SharedConfiguration.Instance.Get<MapConfig>().MaxInstances ?? 10u))
                return false;

            return true;
        }

        /// <summary>
        /// Increase instance count for <see cref="IPlayer"/>.
        /// </summary>
        private void IncreaseInstanceCount(IPlayer player)
        {
            // players with bypass RBAC have no limit to instances in an hour
            if (player.Account.RbacManager.HasPermission(Permission.BypassInstanceLimits))
                return;

            if (instanceCounts.TryGetValue(player.CharacterId, out uint count))
                instanceCounts[player.CharacterId] = count + 1;
            else
                instanceCounts.Add(player.CharacterId, 1);
        }

        /// <summary>
        /// Return a string containing debug information about the map.
        /// </summary>
        public string WriteDebugInformation()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"World Id: {Entry.Id}");
            sb.AppendLine($"Instance Count: {instances.Count}");

            foreach (T instance in instances.Values)
            {
                sb.AppendLine("=====================================");
                sb.AppendLine(instance.WriteDebugInformation());
            }

            return sb.ToString();
        }
    }
}
