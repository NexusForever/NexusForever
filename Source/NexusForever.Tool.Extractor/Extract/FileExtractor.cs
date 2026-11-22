using Microsoft.Extensions.Logging;
using Nexus.Archive;

namespace NexusForever.Tool.Extractor.Extract
{
    public class FileExtractor
    {
        #region Dependency Injection

        private readonly ILogger<FileExtractor> log;
        private readonly Parameters parameters;
        private readonly ArchiveManager archiveManager;

        public FileExtractor(
            ILogger<FileExtractor> log,
            Parameters parameters,
            ArchiveManager archiveManager)
        {
            this.log            = log;
            this.parameters     = parameters;
            this.archiveManager = archiveManager;
        }

        #endregion

        /// <summary>
        /// Extracts a file from the archive to the output path.
        /// </summary>
        public bool Extract(IArchiveFileEntry file, out string filePath, Archive archive = null)
        {
            filePath = Path.Combine(parameters.OutputPath, file.Path.Replace('\\', Path.DirectorySeparatorChar));
            if (File.Exists(filePath))
                return true;

            string directoryPath = Path.GetDirectoryName(filePath);
            Directory.CreateDirectory(directoryPath);

            try
            {
                archive ??= archiveManager.Archive;
                using Stream archiveStream = archive.OpenFileStream(file);
                using Stream fileStream = File.OpenWrite(filePath);
                archiveStream.CopyTo(fileStream);
            }
            catch (Exception e)
            {
                log.LogError(e, "Failed to extract file {filePath}", file.Path);
                return false;
            }

            log.LogTrace("Extracted file {filePath}", file.Path);
            return true;
        }

        /// <summary>
        /// Extracts a file from the archive to the output path.
        /// </summary>
        public bool Extract(string path, out string filePath)
        {
            filePath = Path.Combine(parameters.OutputPath, path.Replace('\\', Path.DirectorySeparatorChar));
            if (File.Exists(filePath))
                return true;

            IArchiveFileEntry propFileEntry = archiveManager.Archive.GetFileInfoByPath(path);
            if (propFileEntry == null)
            {
                log.LogError("Failed to find file {propPath}!", path);
                return false;
            }

            return Extract(propFileEntry, out filePath, archiveManager.Archive);
        }
    }
}
