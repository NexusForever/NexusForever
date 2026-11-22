using Microsoft.Extensions.Logging;
using Nexus.Archive;
using NexusForever.GameTable;

namespace NexusForever.Tool.Extractor.Extract
{
    public class AreaExtractionManager
    {
        #region Dependency Injection

        private readonly ILogger<AreaExtractionManager> log;
        private readonly IGameTableManager gameTableManager;
        private readonly ArchiveManager archiveManager;
        private readonly FileExtractor fileExtractor;

        public AreaExtractionManager(
            ILogger<AreaExtractionManager> log,
            IGameTableManager gameTableManager,
            ArchiveManager archiveManager,
            FileExtractor fileExtractor)
        {
            this.log              = log;
            this.gameTableManager = gameTableManager;
            this.archiveManager   = archiveManager;
            this.fileExtractor    = fileExtractor;
        }

        #endregion

        public void Extract()
        {
            log.LogInformation("Extracting area files...");

            foreach (string assetPath in gameTableManager.World.Entries
                .Where(e => !string.IsNullOrWhiteSpace(e.AssetPath))
                .Select(e => e.AssetPath)
                .Distinct())
            {
                string searchPattern = Path.Combine(assetPath, "*.*.area");
                foreach (IArchiveFileEntry fileEntry in archiveManager.Archive.IndexFile.GetFiles(searchPattern))
                    fileExtractor.Extract(fileEntry, out _);
            }
        }
    }
}
