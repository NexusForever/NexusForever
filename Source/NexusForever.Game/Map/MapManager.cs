using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;
using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Configuration.Model;
using NexusForever.Game.Static.Map;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Static;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;

namespace NexusForever.Game.Map
{
    public sealed class MapManager : Singleton<MapManager>, IMapManager
    {
        private readonly ConcurrentQueue<IPlayerAddToMapContext> pending = [];
        private readonly List<IPlayerAddToMapContext> processing = [];

        private readonly Dictionary</*worldId*/ uint, IMap> maps = [];

        #region Dependency Injection

        private readonly ILogger<MapManager> log;
        private readonly IMapFactory mapFactory;

        public MapManager(
            ILogger<MapManager> log,
            IMapFactory mapFactory)
        {
            this.log        = log;
            this.mapFactory = mapFactory;
        }

        #endregion

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            HandlePendingPlayerAddToMap();
            HandleProcessingPlayerAddToMap();

            UpdateMaps(lastTick);
        }

        private void HandlePendingPlayerAddToMap()
        {
            if (pending.IsEmpty)
                return;

            while (pending.TryDequeue(out IPlayerAddToMapContext context))
            {
                log.LogTrace($"Processing player {context.Player.CharacterId} add to map {context.DestinationPosition.Info.Entry.Id} reuqest.");

                context.Player.ShowLoadingScreen(context.DestinationPosition);

                context.Status = context.Player.Map != null ? PlayerAddToMapStatus.RemoveFromMap : PlayerAddToMapStatus.AddToMap;
                processing.Add(context);
            }
        }

        private void HandleProcessingPlayerAddToMap()
        {
            var toRemove = new List<IPlayerAddToMapContext>();
            foreach (IPlayerAddToMapContext context in processing)
            {
                switch (context.Status)
                {
                    case PlayerAddToMapStatus.RemoveFromMap:
                        RemoveFromMap(context);
                        break;
                    case PlayerAddToMapStatus.AddToMap:
                        AddToMap(context, true);
                        break;
                    case PlayerAddToMapStatus.Complete:
                    {
                        if (context.Error != null)
                            context.OnError?.Invoke(context.Error.Value);

                        log.LogTrace($"Processed player {context.Player.CharacterId} add to map in {(DateTime.Now - context.Time).TotalMilliseconds} ms.");

                        toRemove.Add(context);
                        break;
                    }
                }
            }

            foreach (IPlayerAddToMapContext context in toRemove)
                processing.Remove(context);
        }

        private void RemoveFromMap(IPlayerAddToMapContext context)
        {
            log.LogTrace($"Removing character {context.Player.CharacterId} from map {context.SourcePosition.Info.Entry.Id}.");

            void OnRemoveCallback()
            {
                context.Status = PlayerAddToMapStatus.AddToMap;
            }

            context.Player.RemoveFromMap(OnRemoveCallback);
        }

        private void AddToMap(IPlayerAddToMapContext context, bool destination)
        {
            IMapPosition position = destination ? context.DestinationPosition : context.SourcePosition;

            log.LogTrace($"Adding character {context.Player.CharacterId} to map {position.Info.Entry.Id}.");

            void OnAddCallback(IBaseMap map, uint guid, Vector3 vector)
            {
                context.OnAdd?.Invoke(map, guid, vector);
                context.Status = PlayerAddToMapStatus.Complete;
            }

            void OnErrorCallback(GenericError error)
            {
                log.LogTrace($"An error {error} occured adding player {context.Player.CharacterId} to map!");

                context.Error = error;

                // attempt to add back to previous map
                if (context.Status == PlayerAddToMapStatus.PendingAddToDestinationMap && context.SourcePosition != null)
                    AddToMap(context, false);
                else
                    context.Status = PlayerAddToMapStatus.Complete;
            }

            void OnExceptionCallback(Exception ex)
            {
                log.LogTrace(ex, $"An exception occured adding player {context.Player.CharacterId} to map!");

                context.OnException?.Invoke(ex);
                context.Status = PlayerAddToMapStatus.Complete;
            }

            IMap map = CreateMap(position.Info.Entry);
            switch (map)
            {
                case IBaseMap baseMap:
                    baseMap.EnqueueAdd(context.Player, position.Position, OnAddCallback, OnErrorCallback, OnExceptionCallback);
                    break;
                case IInstancedMap instancedMap:
                    instancedMap.EnqueueAdd(context.Player, position, OnAddCallback, OnErrorCallback, OnExceptionCallback);
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (destination)
                context.Status = PlayerAddToMapStatus.PendingAddToDestinationMap;
            else
            {
                // show the loading screen again for the source map if we failed to add to destination map
                context.Player.ShowLoadingScreen(position);
                context.Status = PlayerAddToMapStatus.PendingAddToSourceMap;
            }
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
            catch (Exception ex)
            {
                log.LogError(ex, "An exception occured during map updates!");
            }

            sw.Stop();
            if (sw.ElapsedMilliseconds > 10)
                log.LogWarning($"{maps.Count} map(s) took {sw.ElapsedMilliseconds}ms to update!");
        }

        /// <summary>
        /// Enqueue <see cref="IPlayer"/> to be added to a map. 
        /// </summary>
        public void AddToMap(IPlayer player, IMapPosition source, IMapPosition destination, OnAddDelegate callback = null, OnGenericErrorDelegate error = null, OnExceptionDelegate exception = null)
        {
            pending.Enqueue(new PlayerAddToMapContext
            {
                Time                = DateTime.Now,
                Player              = player,
                SourcePosition      = source,
                DestinationPosition = destination,
                OnAdd               = callback,
                OnError             = error,
                OnException         = exception
            });
        }

        /// <summary>
        /// Create base <see cref="IMap"/> for <see cref="WorldEntry"/>.
        /// </summary>
        private IMap CreateMap(WorldEntry entry)
        {
            if (maps.TryGetValue(entry.Id, out IMap map))
                return map;

            map = mapFactory.CreateMap(entry.Type);
            map.Initialise(entry);
            maps.Add(entry.Id, map);

            log.LogTrace($"Created new base map for world {entry.Id}.");

            return map;
        }
    }
}
