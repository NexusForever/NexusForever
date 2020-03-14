using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nexus.Archive;
using NexusForever.MapGenerator.GameTable;
using NexusForever.MapGenerator.IO.Area;
using NexusForever.MapGenerator.IO.Map;
using NexusForever.Shared;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.IO;
using NLog;

namespace NexusForever.MapGenerator
{
    public sealed class GenerationManager : Singleton<GenerationManager>
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private GenerationManager()
        {
        }

        public void Initialise()
        {
            log.Info("Generatring base map files...");

            Directory.CreateDirectory("map");
        }

        /// <summary>
        /// Generate a base map (.nfmap) file for a single world optionally specifying a single grid.
        /// </summary>
        public void GenerateWorld(ushort worldId, byte? gridX = null, byte? gridY = null)
        {
            WorldEntry entry = GameTableManager.Instance.World.GetEntry(worldId);
            if (entry != null)
                ProcessWorld(entry, gridX, gridY);
        }

        /// <summary>
        /// Generate base map (.nfmap) files for all worlds.
        /// </summary>
        public void GenerateWorlds(bool singleThread)
        {
            List<Task> taskList = new List<Task>();
            foreach (WorldEntry entry in GameTableManager.Instance.World.Entries
                .Where(e => e.AssetPath != string.Empty)
                .GroupBy(e => e.AssetPath)
                .Select(g => g.First())
                .ToArray())
            {
                if (singleThread)
                    ProcessWorld(entry);
                else
                {
                    Task task = Task.Factory.StartNew(() => ProcessWorld(entry));
                    taskList.Add(task);
                }
            }

            if (!singleThread)
                Task.WaitAll(taskList.ToArray<Task>());
        }

        /// <summary>
        /// Generate a base map (.nfmap) file from supplied <see cref="WorldEntry"/>.
        /// </summary>
        private void ProcessWorld(WorldEntry entry, byte? gridX = null, byte? gridY = null)
        {
            var mapFile = new WritableMapFile(Path.GetFileName(entry.AssetPath));

            log.Info($"Processing {mapFile.Asset}...");

            if (gridX.HasValue && gridY.HasValue)
            {
                string path = Path.Combine(entry.AssetPath, $"{mapFile.Asset}.{gridX:x2}{gridY:x2}.area");
                IArchiveFileEntry grid = ArchiveManager.Instance.MainArchive.GetFileInfoByPath(path);
                if (grid != null)
                    ProcessGrid(mapFile, grid, gridX.Value, gridY.Value);
            }
            else
            {
                string path = Path.Combine(entry.AssetPath, "*.*.area");
                foreach (IArchiveFileEntry grid in ArchiveManager.Instance.MainArchive.IndexFile.GetFiles(path))
                {
                    Regex regex = new Regex(@"[\w]+\.([A-Fa-f0-9]{2})([A-Fa-f0-9]{2})\.area");
                    Match match = regex.Match(grid.FileName);
                    byte x = byte.Parse(match.Groups[1].Value, NumberStyles.HexNumber);
                    byte y = byte.Parse(match.Groups[2].Value, NumberStyles.HexNumber);

                    ProcessGrid(mapFile, grid, x, y);
                }
            }

            // FIXME: this happens for worlds with no terrain information, this is usually an instance where props are used as terrain
            if (!mapFile.Any())
            {
                log.Info($"Map {mapFile.Asset} has no grid information, skipping");
                return;
            }

            // Path.ChangeExtension(mapFile.Asset, "nfmap")
            // ChangeExtension doesn't behave correctly on linux
            string filePath = Path.Combine("map", $"{mapFile.Asset}.nfmap");

            using (FileStream stream = File.Create(filePath))
            using (var writer = new BinaryWriter(stream))
            {
                mapFile.Write(writer);
            }
        }

        private void ProcessGrid(WritableMapFile map, IArchiveFileEntry grid, byte gridX, byte gridY)
        {
            // skip any low quality grids
            if (grid.FileName.Contains("_low", StringComparison.OrdinalIgnoreCase))
                return;

            log.Info($"Processing {map.Asset} grid {gridX},{gridY}...");

            using (Stream stream = ArchiveManager.Instance.MainArchive.OpenFileStream(grid))
            {
                try
                {
                    var mapFileGrid = new WritableMapFileGrid(gridX, gridY);
                    var areaFile = new AreaFile(stream);
                    foreach (IReadable areaChunk in areaFile.Chunks)
                    {
                        switch (areaChunk)
                        {
                            case Chnk chnk:
                            {
                                foreach (ChnkCell cell in chnk.Cells.Where(c => c != null))
                                    mapFileGrid.AddCell(new WritableMapFileCell(cell));
                                break;
                            }
                        }
                    }

                    map.SetGrid(gridX, gridY, mapFileGrid);
                }
                catch (Exception e)
                {
                    log.Error(e);
                }
            }
        }
    }
}
