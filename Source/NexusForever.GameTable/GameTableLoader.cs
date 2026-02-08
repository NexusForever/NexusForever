using NexusForever.Game.Static;

namespace NexusForever.GameTable
{
    public class GameTableLoader
    {
        private readonly List<Type> gameTables = [];
        private readonly List<Language> textTables = [];

        public IEnumerable<Type> GetGameTables()
        {
            return gameTables;
        }

        public IEnumerable<Language> GetTextTables()
        {
            return textTables;
        }

        public GameTableLoader AddTextTable(Language language)
        {
            textTables.Add(language);
            return this;
        }

        public GameTableLoader AddGameTable<T>() where T : class
        {
            gameTables.Add(typeof(T));
            return this;
        }
    }
}
