﻿using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Configuration.Model;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Map;
using NexusForever.Game.Static.RBAC;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Game;
using NLog;

namespace NexusForever.Game.Map
{
    public abstract class MapInstance : BaseMap, IMapInstance
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Unique id for map instance.
        /// </summary>
        /// <remarks>
        /// Map id is unique per map, different world ids can all have instance 1, 2, 3, ect...
        /// </remarks>
        public ulong InstanceId { get; init; }

        /// <summary>
        /// Current unload status for map instance.
        /// </summary>
        /// <remarks>
        /// Status will only be set if the instance is in an unloading state.
        /// </remarks>
        public MapUnloadStatus? UnloadStatus
        {
            get => unloadStatus;
            private set
            {
                log.Trace($"Unloading map:{Entry.Id}, instance:{InstanceId}, state: {value}");
                unloadStatus = value;
            }
        }
        private MapUnloadStatus? unloadStatus;

        private IMapPosition unloadPosition;

        private uint instanceLimit;

        private readonly UpdateTimer unloadTimer 
            = new(SharedConfiguration.Instance.Get<MapConfig>().GridUnloadTimer ?? 600d);

        private readonly HashSet<uint> playerEntities = new();
        private readonly Dictionary<uint, IMapInstanceRemoval> instanceRemovals = new();

        /// <summary>
        /// Initialise <see cref="IMapInstance"/> with <see cref="WorldEntry"/>.
        /// </summary>
        public override void Initialise(WorldEntry entry)
        {
            base.Initialise(entry);

            // TODO: find where this should come from, this is just an arbitrary value
            instanceLimit = 100u;
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public override void Update(double lastTick)
        {
            base.Update(lastTick);

            ProcessInstanceRemovals(lastTick);
            ProcessUnload(lastTick);
        }

        private void ProcessInstanceRemovals(double lastTick)
        {
            if (instanceRemovals.Count == 0)
                return;

            var remove = new List<uint>();
            foreach (IMapInstanceRemoval removal in instanceRemovals.Values)
            {
                removal.Timer -= lastTick;
                if (removal.Timer > 0d)
                    continue;

                // remove player from instance and teleport to new location
                IPlayer player = GetEntity<IPlayer>(removal.Guid);
                player?.TeleportTo(removal.Position);

                remove.Add(removal.Guid);
            }

            foreach (uint guid in remove)
                instanceRemovals.Remove(guid);
        }

        private void ProcessUnload(double lastTick)
        {
            switch (UnloadStatus)
            {
                case MapUnloadStatus.AwaitingGridActions:
                    ProcessUnloadAwaitingGridActions();
                    break;
                case MapUnloadStatus.AwaitingUnloadPlayers:
                    ProcessUnloadAwaitingUnloadPlayers();
                    break;
                case MapUnloadStatus.UnloadingPlayers:
                    ProcessUnloadUnloadingPlayers();
                    break;
                case MapUnloadStatus.AwaitingUnloadEntities:
                    ProcessUnloadAwaitingUnloadEntities();
                    break;
                case MapUnloadStatus.UnloadingEntities:
                    ProcessUnloadUnloadingEntities();
                    break;
                case null:
                {
                    unloadTimer.Update(lastTick);
                    if (unloadTimer.HasElapsed)
                    {
                        unloadTimer.Reset(false);
                        Unload();
                    }

                    break;
                }
            }
        }

        private void ProcessUnloadAwaitingGridActions()
        {
            // waiting for pending grid actions to be processed
            if (pendingActions.Count == 0)
                UnloadStatus = MapUnloadStatus.AwaitingUnloadPlayers;
        }

        private void ProcessUnloadAwaitingUnloadPlayers()
        {
            // unload players from instance and move them to return positions
            foreach (uint guid in playerEntities)
            {
                IPlayer player = GetEntity<IPlayer>(guid);
                if (player != null)
                {
                    IMapPosition position = unloadPosition ?? GetPlayerReturnLocation(player);
                    player.TeleportTo(position, TeleportReason.Unload);
                }
            }

            unloadPosition = null;
            UnloadStatus = MapUnloadStatus.UnloadingPlayers;
        }

        private void ProcessUnloadUnloadingPlayers()
        {
            // waiting for players to be removed from instance
            if (playerEntities.Count == 0)
                UnloadStatus = MapUnloadStatus.AwaitingUnloadEntities;
        }

        private void ProcessUnloadAwaitingUnloadEntities()
        {
            // unload grids from instance
            foreach (IMapGrid grid in GetActiveGrids())
                grid.Unload();

            UnloadStatus = MapUnloadStatus.UnloadingEntities;
        }

        private void ProcessUnloadUnloadingEntities()
        {
            // waiting for entities to be removed from instance
            if (entities.Count == 0)
                UnloadStatus = MapUnloadStatus.Complete;
        }

        /// <summary>
        /// Returns if <see cref="IGridEntity"/> can be added to <see cref="IBaseMap"/>.
        /// </summary>
        public override GenericError? CanEnter(IPlayer player, IMapPosition position)
        {
            // elevated users bypass instance player limits
            if (!player.Account.RbacManager.HasPermission(Permission.BypassInstanceLimits))
            {
                int count = playerEntities
                    // include players without bypass permission
                    .Select(GetEntity<IPlayer>)
                    .Count(p => !p.Account.RbacManager.HasPermission(Permission.BypassInstanceLimits))
                    // include players pending add to instance
                    + pendingActions.Count(a => a is IGridActionAdd or IGridActionPending);

                if (count >= instanceLimit)
                    return GenericError.InstanceFull;
            }

            return base.CanEnter(player, position);
        }

        protected override void AddEntity(IGridEntity entity, Vector3 vector)
        {
            base.AddEntity(entity, vector);
            if (entity is IPlayer player)
            {
                playerEntities.Add(player.Guid);

                // stop map unload timer when a player is added to map
                if (unloadTimer.IsTicking)
                    unloadTimer.Pause();
            }
        }

        protected override void RemoveEntity(IGridEntity entity)
        {
            if (entity is IPlayer player)
            {
                // cancel any pending removals if the player is removed from the map prematurely
                instanceRemovals.Remove(player.Guid);
                playerEntities.Remove(player.Guid);

                // start map unload timer when the last player is removed
                if (playerEntities.Count == 0)
                    unloadTimer.Reset();
            }

            base.RemoveEntity(entity);
        }

        /// <summary>
        /// Start unloading map instance.
        /// </summary>
        /// <remarks>
        /// Any <see cref="IPlayer"/>'s still in the instance will be moved to return locations.
        /// </remarks>
        public void Unload(IMapPosition unloadPosition = null)
        {
            if (UnloadStatus.HasValue)
                throw new InvalidOperationException("Failed to start instance unload, a pending unload is already in progress!");

            this.unloadPosition = unloadPosition;

            UnloadStatus = MapUnloadStatus.AwaitingGridActions;
            OnUnload();
        }

        protected virtual void OnUnload()
        {
            // deliberately empty
        }

        protected abstract IMapPosition GetPlayerReturnLocation(IPlayer player);

        /// <summary>
        /// Enqueue <see cref="IPlayer"/> to be removed from map for <see cref="WorldRemovalReason"/>.
        /// After a short duration the <see cref="IPlayer"/> will be teleported their return location.
        /// </summary>
        public void EnqueuePendingRemoval(IPlayer player, WorldRemovalReason reason)
        {
            if (instanceRemovals.ContainsKey(player.Guid))
                return;

            GameFormulaEntry entry = GameTableManager.Instance.GameFormula.GetEntry(1123);
            var removal = new MapInstanceRemoval
            {
                Guid     = player.Guid,
                Reason   = reason,
                Timer    = (entry?.Dataint0 ?? 30000d) / 1000d, // 30 seconds is what client defaults to if game formula is missing
                Position = GetPlayerReturnLocation(player)
            };

            instanceRemovals.Add(player.Guid, removal);

            player.Session.EnqueueMessageEncrypted(new ServerPendingWorldRemoval
            {
                Reason = reason
            });
        }

        /// <summary>
        /// Cancel any pending removal for <see cref="IPlayer"/>.
        /// </summary>
        public void CancelPendingRemoval(IPlayer player)
        {
            if (!instanceRemovals.Remove(player.Guid))
                return;

            player.Session.EnqueueMessageEncrypted(new ServerPendingWorldRemovalCancel());
        }
    }
}
