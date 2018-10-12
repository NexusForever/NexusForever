using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NLog;

namespace NexusForever.WorldServer.Game.Map
{
    public static class MapManager
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private static readonly Dictionary</*worldId*/ ushort, BaseMap> maps = new Dictionary<ushort, BaseMap>();

        /// <summary>
        /// Enqueue <see cref="GridEntity"/> to be added to a map. 
        /// </summary>
        public static void AddToMap(GridEntity entity, ushort worldId, Vector3 vector3)
        {
            WorldEntry entry = GameTableManager.World.GetEntry(worldId);
            if (entry == null)
                throw new ArgumentException();

            if (maps.TryGetValue(worldId, out BaseMap map))
                map.EnqueueAdd(entity, vector3);
            else
            {
                var newMap = new BaseMap(entry);
                newMap.EnqueueAdd(entity, vector3);
                maps.Add(worldId, newMap);
            }
        }

		/// <summary>
		/// Appended by Xan.
		/// Move an existing <see cref="GridEntity"/> to another location on the map.
		/// </summary>
		public static void MoveExistingInMap(GridEntity entity, ushort worldId, Vector3 vector3)
		{
			WorldEntry entry = GameTableManager.World.GetEntry(worldId);
			if (entry == null)
				throw new ArgumentException();

			if (maps.TryGetValue(worldId, out BaseMap map))
				map.EnqueueRelocate(entity, vector3);
			else {
				var newMap = new BaseMap(entry);
				newMap.EnqueueRelocate(entity, vector3);
				maps.Add(worldId, newMap);
			}
		}

		public static void Update(double lastTick)
        {
            if (maps.Count == 0)
                return;

            var sw = Stopwatch.StartNew();

            var tasks = new List<Task>();
            foreach (BaseMap map in maps.Values)
                tasks.Add(Task.Run(() => { map.Update(lastTick); }));

            Task.WaitAll(tasks.ToArray());

            sw.Stop();
            if (sw.ElapsedMilliseconds > 10)
                log.Warn($"{maps.Count} map(s) took {sw.ElapsedMilliseconds} to update!");
        }
    }
}
