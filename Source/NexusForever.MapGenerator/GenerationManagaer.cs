using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Nexus.Archive;
using NexusForever.MapGenerator.GameTable;
using NexusForever.MapGenerator.IO.Area;
using NexusForever.MapGenerator.IO.Map;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.IO;
using NLog;

namespace NexusForever.MapGenerator
{
    public static class GenerationManagaer
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public static void Initialise()
        {
            log.Info("Generatring base map files...");

            Directory.CreateDirectory("map");

            foreach (WorldEntry entry in GameTableManager.World.Entries
                .Where(e => e.AssetPath != string.Empty)
                .GroupBy(e => e.AssetPath)
                .Select(g => g.First())
                .ToArray())
            {
                ProcessWorld(entry);
            }
        }

        /// <summary>
        /// Generate a base map (.nfmap) file from supplied <see cref="WorldEntry"/>.
        /// </summary>
        private static void ProcessWorld(WorldEntry entry)
        {
            var mapFile = new WritableMapFile(Path.GetFileName(entry.AssetPath));

            log.Info($"Processing {mapFile.Asset}...");

            foreach (IArchiveFileEntry fileEntry in ArchiveManager.MainArchive.IndexFile.GetFiles(Path.Combine(entry.AssetPath, "*.*.area")))
            {
                // skip any low quality grids
                if (fileEntry.FileName.Contains("_low", StringComparison.OrdinalIgnoreCase))
                    continue;

                Regex regex = new Regex(@"[\w]+\.([A-Fa-f0-9]{2})([A-Fa-f0-9]{2})\.area");
                Match match = regex.Match(fileEntry.FileName);
                uint x = uint.Parse(match.Groups[1].Value, NumberStyles.HexNumber);
                uint y = uint.Parse(match.Groups[2].Value, NumberStyles.HexNumber);

                log.Info($"Processing {mapFile.Asset} grid {x},{y}...");

                using (Stream stream = ArchiveManager.MainArchive.OpenFileStream(fileEntry))
                {
                    try
                    {
                        var mapFileGrid = new WritableMapFileGrid(x, y);

                        var areaFile = new AreaFile(stream);
                        foreach (IReadable areaChunk in areaFile.Chunks)
                        {
                            switch (areaChunk)
                            {
                                case Chnk chnk:
                                {
                                    foreach (ChnkCell cell in chnk.Cells.Where(c => c != null))
                                    {
                                        var mapFileCell = new WritableMapFileCell(cell);
                                        mapFileGrid.AddCell(mapFileCell);
                                    }
                                    break;
                                }
                            }
                        }

                        mapFile.SetGrid(x, y, mapFileGrid);
                    }
                    catch (Exception e)
                    {
                        log.Error(e);
                    }
                }
            }

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
    }
}
