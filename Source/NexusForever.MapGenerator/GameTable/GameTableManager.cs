using System.IO;
using Nexus.Archive;
using NexusForever.Shared;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;

namespace NexusForever.MapGenerator.GameTable
{
    public sealed class GameTableManager : Singleton<GameTableManager>
    {
        public GameTable<WorldEntry> World { get; private set; }

        private GameTableManager()
        {
        }

        public void Initialise()
        {
            World = LoadGameTable<WorldEntry>("World.tbl");
        }

        /// <summary>
        /// Return <see cref="GameTable{T}"/> for supplied table name found in the main client archive.
        /// </summary>
        private GameTable<T> LoadGameTable<T>(string name) where T : class, new()
        {
            string filePath = Path.Combine("DB", name);
            if (!(ArchiveManager.Instance.MainArchive.IndexFile.FindEntry(filePath) is IArchiveFileEntry file))
                throw new FileNotFoundException();

            using (Stream archiveStream = ArchiveManager.Instance.MainArchive.OpenFileStream(file))
            using (var memoryStream = new MemoryStream())
            {
                archiveStream.CopyTo(memoryStream);
                memoryStream.Position = 0;
                return new GameTable<T>(memoryStream);
            }
        }
    }
}
