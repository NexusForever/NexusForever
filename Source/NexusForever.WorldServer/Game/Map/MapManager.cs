using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Game;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Map.Static;
using NexusForever.WorldServer.Game.Static;
using NLog;

namespace NexusForever.WorldServer.Game.Map
{
    public sealed class MapManager : Singleton<MapManager>, IUpdate
    {
        private class PendingAdd
        {
            public Player Player { get; init; }
            public MapPosition MapPosition { get; init; }
        }

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentQueue<PendingAdd> pendingAdds = new();
        private readonly Dictionary</*worldId*/ ushort, IMap> maps = new();

        private readonly ConcurrentDictionary<ulong, uint> instanceCounts = new();
        // reset instance limit counts every hour
        private readonly UpdateTimer instanceCountReset = new(TimeSpan.FromMinutes(60));

        private MapManager()
        {
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

                if (map is IInstancedMap instancedMap)
                    instancedMap.EnqueueAdd(pending.Player, pending.MapPosition);
                else
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
                if (ConfigurationManager<WorldServerConfiguration>.Instance.Config.Map.SynchronousUpdate)
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
        /// Enqueue <see cref="Player"/> to be added to a map. 
        /// </summary>
        public void AddToMap(Player player, MapPosition mapPosition)
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
        /// Returns if <see cref="Player"/> can create a new instance.
        /// </summary>
        public bool CanCreateInstance(Player player)
        {
            if (instanceCounts.TryGetValue(player.CharacterId, out uint instanceCount)
                && instanceCount > (ConfigurationManager<WorldServerConfiguration>.Instance.Config.Map.MaxInstances ?? 10u))
                return false;

            return true;
        }

        /// <summary>
        /// Increase instance count for <see cref="Player"/>.
        /// </summary>
        public void IncreaseInstanceCount(Player player)
        {
            // players with bypass RBAC have no limit to instances in an hour
            if (player.Session.AccountRbacManager.HasPermission(RBAC.Static.Permission.BypassInstanceLimits))
                return;

            instanceCounts.AddOrUpdate(player.CharacterId, 1, (k, v) => v + 1);
        }
    }
}
