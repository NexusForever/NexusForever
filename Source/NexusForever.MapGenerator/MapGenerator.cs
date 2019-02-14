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
    internal static class MapGenerator
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        #if DEBUG
        private const string Title = "NexusForever: Map Generator (DEBUG)";
        #else
        private const string Title = "NexusForever: Map Generator (RELEASE)";
        #endif

        private static void Main(string[] args)
        {
            Console.Title = Title;

            if (!Directory.Exists(args[0]))
                throw new DirectoryNotFoundException();

            Directory.CreateDirectory("output");

            ArchiveFile coreDataArchive = null;
            if (File.Exists(Path.Combine(args[0], "CoreData.archive")))
                coreDataArchive = ArchiveFileBase.FromFile(Path.Combine(args[0], "CoreData.archive")) as ArchiveFile;

            Archive archive = Archive.FromFile(Path.Combine(args[0], "ClientData.index"), coreDataArchive);

            GameTableManager.Initialise(archive);

            foreach (WorldEntry entry in GameTableManager.World.Entries
                .Where(e => e.AssetPath != string.Empty)
                .GroupBy(e => e.AssetPath)
                .Select(g => g.First())
                .ToArray())
            {
                ProcessWorld(archive, entry);
            }

            log.Info("Finished");
            Console.ReadLine();
        }

        private static void ProcessWorld(Archive archive, WorldEntry entry)
        {
            var mapFile = new WritableMapFile(Path.GetFileName(entry.AssetPath));

            Console.Title = $"{Title} - Processing {mapFile.Asset}...";
            log.Info($"Processing {mapFile.Asset}...");

            foreach (IArchiveFileEntry fileEntry in archive.IndexFile.GetFiles(Path.Combine(entry.AssetPath, "*.*.area")))
            {
                // skip any low quality grids
                if (fileEntry.FileName.Contains("_low", StringComparison.OrdinalIgnoreCase))
                    continue;

                Regex regex = new Regex(@"[\w]+\.([A-Fa-f0-9]{2})([A-Fa-f0-9]{2})\.area");
                Match match = regex.Match(fileEntry.FileName);
                uint x = uint.Parse(match.Groups[1].Value, NumberStyles.HexNumber);
                uint y = uint.Parse(match.Groups[2].Value, NumberStyles.HexNumber);

                log.Info($"Processing grid {x},{y}...");

                using (Stream stream = archive.OpenFileStream(fileEntry))
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

            string filePath = Path.Combine("output", Path.ChangeExtension(mapFile.Asset, "nfmap"));
            using (FileStream stream = File.Create(filePath))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                mapFile.Write(writer);
            }
        }
    }
}
