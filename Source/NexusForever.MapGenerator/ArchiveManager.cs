using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nexus.Archive;
using NexusForever.Shared;
using NLog;

namespace NexusForever.MapGenerator
{
    public sealed class ArchiveManager : Singleton<ArchiveManager>
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private static readonly string[] localisationIndexes =
        {
            "ClientDataEN.index",
            "ClientDataFR.index",
            "ClientDataDE.index"
        };

        /// <summary>
        /// Main client ClientData archive.
        /// </summary>
        public Archive MainArchive { get; private set; }

        /// <summary>
        /// Collection of client localisation archives.
        /// </summary>
        public List<Archive> LocalisationArchives { get; } = new List<Archive>();

        public void Initialise(string patchPath)
        {
            log.Info("Loading archives...");

            // CoreData archive only applicable to Steam client
            ArchiveFile coreDataArchive = null;
            if (File.Exists(Path.Combine(patchPath, "CoreData.archive")))
                coreDataArchive = ArchiveFileBase.FromFile(Path.Combine(patchPath, "CoreData.archive")) as ArchiveFile;

            MainArchive = Archive.FromFile(Path.Combine(patchPath, "ClientData.index"), coreDataArchive);

            foreach (string localisationArchivePath in localisationIndexes
                .Select(i => Path.Combine(patchPath, i)))
            {
                if (!File.Exists(localisationArchivePath))
                    continue;

                LocalisationArchives.Add(Archive.FromFile(localisationArchivePath, coreDataArchive));
            }
        }
    }
}
