using System.IO;
using Nexus.Archive;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;

namespace NexusForever.MapGenerator.GameTable
{
    public static class GameTableManager
    {
        public static GameTable<WorldEntry> World { get; private set; }

        public static void Initialise(Archive archive)
        {
            World = LoadGameTable<WorldEntry>(archive, "DB\\World.tbl");
        }

        private static GameTable<T> LoadGameTable<T>(Archive archive, string path) where T : class, new()
        {
            if (!(archive.IndexFile.FindEntry(path) is IArchiveFileEntry file))
                throw new FileNotFoundException();

            using (Stream stream = archive.OpenFileStream(file))
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                memoryStream.Position = 0;
                return new GameTable<T>(memoryStream);
            }
        }
    }
}
