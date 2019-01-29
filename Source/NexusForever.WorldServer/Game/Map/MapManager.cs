using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.IO.Map;
using NexusForever.WorldServer.Game.Entity;
using NLog;

namespace NexusForever.WorldServer.Game.Map
{
    public static class MapManager
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private static readonly Dictionary</*worldId*/ ushort, IMap> maps = new Dictionary<ushort, IMap>();

        public static void Initialise()
        {
            ValidateMapFiles();
        }

        private static void ValidateMapFiles()
        {
            log.Info("Validating map files...");

            string mapPath = ConfigurationManager<WorldServerConfiguration>.Config.Map.MapPath;
            if (!Directory.Exists(mapPath))
                throw new DirectoryNotFoundException();

            foreach (string fileName in Directory.EnumerateFiles(mapPath, "*.nfmap"))
            {
                using (FileStream stream = File.OpenRead(fileName))
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    var mapFile = new MapFile();
                    mapFile.ReadHeader(reader);
                }
            }
        }

        /// <summary>
        /// Enqueue <see cref="Player"/> to be added to a map. 
        /// </summary>
        public static void AddToMap(Player player, MapInfo info, Vector3 vector3)
        {
            if (info?.Entry == null)
                throw new ArgumentException();

            IMap map = CreateMap(info, player);
            map.EnqueueAdd(player, vector3);
        }

        /// <summary>
        /// Create base or instanced <see cref="IMap"/> of <see cref="MapInfo"/> for <see cref="Player"/>.
        /// </summary>
        private static IMap CreateMap(MapInfo info, Player player)
        {
            IMap map = CreateBaseMap(info);
            if (map is IInstancedMap iMap)
                map = iMap.CreateInstance(info, player);

            return map;
        }

        /// <summary>
        /// Create and store base <see cref="IMap"/> of <see cref="MapInfo"/>.
        /// </summary>
        private static IMap CreateBaseMap(MapInfo info)
        {
            if (maps.TryGetValue((ushort)info.Entry.Id, out IMap map))
                return map;

            switch (info.Entry.Type)
            {
                case 5:
                    map = new InstancedMap<ResidenceMap>();
                    break;
                default:
                    map = new BaseMap();
                    break;
            }
            
            map.Initialise(info, null);
            maps.Add((ushort)info.Entry.Id, map);
            return map;
        }

        public static void Update(double lastTick)
        {
            if (maps.Count == 0)
                return;

            var sw = Stopwatch.StartNew();

            var tasks = new List<Task>();
            foreach (IMap map in maps.Values)
                tasks.Add(Task.Run(() => { map.Update(lastTick); }));
            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch
            {
                // ignored.
            }

            sw.Stop();
            if (sw.ElapsedMilliseconds > 10)
                log.Warn($"{maps.Count} map(s) took {sw.ElapsedMilliseconds}ms to update!");
        }
    }
}
