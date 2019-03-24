using System.IO;
using System.Linq;
using Nexus.Archive;
using NLog;

namespace NexusForever.MapGenerator
{
    public static class ExtractionManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private static readonly string[] languageFiles =
        {
            "en-US.bin",
            "de-DE.bin",
            "fr-FR.bin",
            "en-GB.bin"
        };

        public static void Initialise()
        {
            log.Info("Extracting GameTables...");

            Directory.CreateDirectory("tbl");

            ExtractGameTables();
            ExtractLanguageFiles();
        }

        /// <summary>
        /// Extract all GameTables (*.tbl) from main client archive.
        /// </summary>
        private static void ExtractGameTables()
        {
            string searchPattern = Path.Combine("DB", "*.tbl");
            foreach (IArchiveFileEntry fileEntry in ArchiveManager.MainArchive.IndexFile.GetFiles(searchPattern))
                ExtractFile(ArchiveManager.MainArchive, fileEntry);
        }

        /// <summary>
        /// Extract all language files (*.bin) from all present localisation client archives.
        /// </summary>
        private static void ExtractLanguageFiles()
        {
            foreach (Archive archive in ArchiveManager.LocalisationArchives)
            {
                foreach (IArchiveFileEntry fileEntry in languageFiles
                    .Select(archive.IndexFile.FindEntry)
                    .OfType<IArchiveFileEntry>())
                {
                    ExtractFile(archive, fileEntry);
                }
            }
        }

        /// <summary>
        /// Extract supplied <see cref="IArchiveFileEntry"/> from <see cref="Archive"/>.
        /// </summary>
        private static void ExtractFile(Archive archive, IArchiveFileEntry fileEntry)
        {
            string filePath = Path.Combine("tbl", fileEntry.FileName);

            using (Stream archiveStream = archive.OpenFileStream(fileEntry))
            using (FileStream fileStream = File.OpenWrite(filePath))
            {
                archiveStream.CopyTo(fileStream);
            }

            log.Info($"Extracted {fileEntry.FileName}...");
        }
    }
}
