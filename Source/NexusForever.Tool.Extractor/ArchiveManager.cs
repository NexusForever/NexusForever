using Microsoft.Extensions.Logging;
using Nexus.Archive;

namespace NexusForever.Tool.Extractor
{
    public class ArchiveManager
    {
        private static readonly string[] localisationIndexes =
        {
            "ClientDataEN.index",
            "ClientDataFR.index",
            "ClientDataDE.index"
        };

        /// <summary>
        /// Main client ClientData archive.
        /// </summary>
        public Archive Archive { get; private set; }

        /// <summary>
        /// Collection of client localisation archives.
        /// </summary>
        public List<Archive> LocalisationArchives { get; } = [];

        #region Dependency Injection

        private readonly ILogger<ArchiveManager> log;
        private readonly Parameters parameters;

        public ArchiveManager(
            ILogger<ArchiveManager> log,
            Parameters parameters)
        {
            this.log        = log;
            this.parameters = parameters;
        }

        #endregion

        public void Initialise()
        {
            // CoreData archive only applicable to Steam client
            ArchiveFile coreDataArchive = null;
            if (File.Exists(Path.Combine(parameters.PatchPath, "CoreData.archive")))
                coreDataArchive = ArchiveFileBase.FromFile(Path.Combine(parameters.PatchPath, "CoreData.archive")) as ArchiveFile;

            string path = Path.Combine(parameters.PatchPath, "ClientData.index");
            Archive = Archive.FromFile(path, coreDataArchive);

            log.LogInformation("Loaded archive {path}", path);

            foreach (string localisationFile in localisationIndexes)
            {
                string localisationPath = Path.Combine(parameters.PatchPath, localisationFile);
                if (!File.Exists(localisationPath))
                    continue;

                Archive localisationArchive = Archive.FromFile(localisationPath, coreDataArchive);
                LocalisationArchives.Add(localisationArchive);

                log.LogInformation("Loaded localisation archive {localisationPath}", localisationPath);
            }
        }
    }
}
