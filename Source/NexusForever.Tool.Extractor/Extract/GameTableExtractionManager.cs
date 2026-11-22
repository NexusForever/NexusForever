using Microsoft.Extensions.Logging;
using Nexus.Archive;

namespace NexusForever.Tool.Extractor.Extract
{
    public class GameTableExtractionManager
    {
        private static readonly string[] languageFiles =
        {
            "en-US.bin",
            "de-DE.bin",
            "fr-FR.bin",
            "en-GB.bin"
        };

        #region Dependency Injection

        private readonly ILogger<GameTableExtractionManager> log;
        private readonly ArchiveManager archiveManager;
        private readonly FileExtractor fileExtractor;

        public GameTableExtractionManager(
            ILogger<GameTableExtractionManager> log,
            ArchiveManager archiveManager,
            FileExtractor fileExtractor)
        {
            this.log            = log;
            this.archiveManager = archiveManager;
            this.fileExtractor  = fileExtractor;
        }

        #endregion

        public void Extract()
        {
            ExtractGameTables();
            ExtractLanguageFiles();
        }

        /// <summary>
        /// Extract all GameTables (*.tbl) from main client archive.
        /// </summary>
        private void ExtractGameTables()
        {
            log.LogInformation("Extracting game tables...");

            string searchPattern = Path.Combine("DB", "*.tbl");
            foreach (IArchiveFileEntry fileEntry in archiveManager.Archive.IndexFile.GetFiles(searchPattern))
                fileExtractor.Extract(fileEntry, out _);
        }

        /// <summary>
        /// Extract all language files (*.bin) from all present localisation client archives.
        /// </summary>
        private void ExtractLanguageFiles()
        {
            log.LogInformation("Extracting language files...");

            foreach (Archive archive in archiveManager.LocalisationArchives)
            {
                foreach (string languageFile in languageFiles)
                {
                    IArchiveFileEntry fileEntry = archive.GetFileInfoByPath(languageFile);
                    if (fileEntry != null)
                        fileExtractor.Extract(fileEntry, out _, archive);
                }
            }
        }
    }
}
