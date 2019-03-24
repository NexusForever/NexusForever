using System.IO;
using Nexus.Archive;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;

namespace NexusForever.MapGenerator.GameTable
{
    public static class GameTableManager
    {
        public static GameTable<WorldEntry> World { get; private set; }

        public static void Initialise()
        {
            World = LoadGameTable<WorldEntry>("World.tbl");
        }

        /// <summary>
        /// Return <see cref="GameTable{T}"/> for supplied table name found in the main client archive.
        /// </summary>
        private static GameTable<T> LoadGameTable<T>(string name) where T : class, new()
        {
            string filePath = Path.Combine("DB", name);
            if (!(ArchiveManager.MainArchive.IndexFile.FindEntry(filePath) is IArchiveFileEntry file))
                throw new FileNotFoundException();

            using (Stream archiveStream = ArchiveManager.MainArchive.OpenFileStream(file))
            using (var memoryStream = new MemoryStream())
            {
                archiveStream.CopyTo(memoryStream);
                memoryStream.Position = 0;
                return new GameTable<T>(memoryStream);
            }
        }
    }
}
