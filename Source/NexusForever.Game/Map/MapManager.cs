using System.Collections.Concurrent;
using System.Diagnostics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Configuration.Model;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Map;
using NexusForever.Game.Static.RBAC;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Static;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Game;
using NLog;

namespace NexusForever.Game.Map
{
    public sealed class MapManager : Singleton<MapManager>, IMapManager
    {
        private class PendingAdd
        {
            public IPlayer Player { get; init; }
            public IMapPosition MapPosition { get; init; }
        }

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentQueue<PendingAdd> pendingAdds = new();
        private readonly Dictionary</*worldId*/ ushort, IMap> maps = new();

        private readonly ConcurrentDictionary<ulong, uint> instanceCounts = new();
        // reset instance limit counts every hour
        private readonly UpdateTimer instanceCountReset = new(TimeSpan.FromMinutes(60));

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
            UpdateMaps(lastTick);
        }

        private void ProcessPending()
        {
            if (pendingAdds.Count == 0)
                return;

            var sw = Stopwatch.StartNew();

            while (pendingAdds.TryDequeue(out PendingAdd pending))
            {
                IMap map = CreateMap(pending.MapPosition.Info.Entry);

                GenericError? result = map.CanEnter(pending.Player, pending.MapPosition);
                if (result.HasValue)
                {
                    pending.Player.OnTeleportToFailed(result.Value);
                    return;
                }

                map.EnqueueAdd(pending.Player, pending.MapPosition);
            }

            sw.Stop();
            if (sw.ElapsedMilliseconds > 10)
                log.Warn($"{pendingAdds.Count} pending add(s) took {sw.ElapsedMilliseconds}ms to process!");
        }

        private void UpdateMaps(double lastTick)
        {
            if (maps.Count == 0)
                return;

            var sw = Stopwatch.StartNew();

            try
            {
                if (SharedConfiguration.Instance.Get<MapConfig>().SynchronousUpdate)
                {
                    foreach (IMap map in maps.Values)
                        map.Update(lastTick);
                }
                else
                {
                    var tasks = new List<Task>();
                    foreach (IMap map in maps.Values)
                        tasks.Add(Task.Run(() => { map.Update(lastTick); }));

                    Task.WaitAll(tasks.ToArray());
                }
            }
            catch
            {
                // ignored.
            }

            sw.Stop();
            if (sw.ElapsedMilliseconds > 10)
                log.Warn($"{maps.Count} map(s) took {sw.ElapsedMilliseconds}ms to update!");
        }

        /// <summary>
        /// Enqueue <see cref="IPlayer"/> to be added to a map. 
        /// </summary>
        public void AddToMap(IPlayer player, IMapPosition mapPosition)
        {
            pendingAdds.Enqueue(new PendingAdd
            {
                Player      = player,
                MapPosition = mapPosition
            });
        }

        /// <summary>
        /// Create base <see cref="IMap"/> for <see cref="WorldEntry"/>.
        /// </summary>
        private IMap CreateMap(WorldEntry entry)
        {
            if (maps.TryGetValue((ushort)entry.Id, out IMap map))
                return map;

            switch ((MapType)entry.Type)
            {
                case MapType.Residence:
                case MapType.Community:
                    map = new ResidenceInstancedMap();
                    break;
                default:
                    map = new BaseMap();
                    break;
            }

            map.Initialise(entry);
            maps.Add((ushort)entry.Id, map);

            log.Trace($"Created new base map for world {entry.Id}.");

            return map;
        }

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can create a new instance.
        /// </summary>
        public bool CanCreateInstance(IPlayer player)
        {
            if (instanceCounts.TryGetValue(player.CharacterId, out uint instanceCount)
                && instanceCount > (SharedConfiguration.Instance.Get<MapConfig>().MaxInstances ?? 10u))
                return false;

            return true;
        }

        /// <summary>
        /// Increase instance count for <see cref="IPlayer"/>.
        /// </summary>
        public void IncreaseInstanceCount(IPlayer player)
        {
            // players with bypass RBAC have no limit to instances in an hour
            if (player.Account.RbacManager.HasPermission(Permission.BypassInstanceLimits))
                return;

            instanceCounts.AddOrUpdate(player.CharacterId, 1, (k, v) => v + 1);
        }

        /// <summary>
        /// Returns the <see cref="RezType"/> mask allowed for the <see cref="IPlayer"/>'s Map.
        /// </summary>
        public RezType GetRezTypeForMap(IPlayer player)
        {
            if (player.Map == null)
                return RezType.None;

            // TODO: Add support for all map type RezType's
            return RezType.OpenWorld;
        }
    }
}
