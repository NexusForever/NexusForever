using System.Collections.Immutable;
using System.Reflection;
using NexusForever.Game.Abstract.Text.Search;
using NexusForever.Game.Static;
using NexusForever.GameTable;
using NexusForever.Shared;
using NLog;

namespace NexusForever.Game.Text.Search
{
    /// <summary>
    /// Responsible for looking up text and objects that the text references.
    /// </summary>
    public sealed class SearchManager : Singleton<SearchManager>, ISearchManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private ImmutableDictionary<Language, ITextReverseIndex> reverseIndexDictionary;

        public void Initialise()
        {
            log.Info("Creating reverse text lookups.");
            var index = new Dictionary<Language, ITextReverseIndex>
            {
                [Language.English] = new TextReverseIndex(GameTableManager.Instance.TextEnglish),
                [Language.French]  = new TextReverseIndex(GameTableManager.Instance.TextFrench),
                [Language.German]  = new TextReverseIndex(GameTableManager.Instance.TextGerman)
            };
            reverseIndexDictionary = index.ToImmutableDictionary();
            foreach ((Language language, ITextReverseIndex value) in index)
            {
                if (value.IsEmpty)
                    log.Warn($"Language {language} was not loaded, and will not be used for text search");
                else
                    log.Debug($"Language {language} loaded.");
            }
        }

        private GameTable<T> GetGameTable<T>() where T : class, new()
        {
            return GameTableManager.Instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(i => i.PropertyType == typeof(GameTable<T>))
                .Select(property => property.GetValue(GameTableManager.Instance) as GameTable<T>)
                .FirstOrDefault();
        }

        /// <summary>
        /// Search for text ID's
        /// </summary>
        /// <param name="text"></param>
        /// <param name="language"></param>
        /// <param name="fuzzy"></param>
        /// <returns></returns>
        public IEnumerable<uint> Search(string text, Language language, bool fuzzy = false)
        {
            ITextReverseIndex index = GetIndex(language);
            if (index == null)
                return Enumerable.Empty<uint>();
            List<uint> ids = new();
            if (fuzzy)
                ids.AddRange(index.FuzzySearch(text));
            else
                ids.AddRange(index.ExactSearch(text));

            return ids;
        }

        public IEnumerable<T> Search<T>(string text, Language language,
            Func<T, uint> textIdAccessor,
            bool fuzzy = false) where T : class, new()
        {
            return Search<T>(text, language, t => new[] { textIdAccessor(t) }, fuzzy);
        }

        /// <summary>
        /// Find objects matching the specified search string
        /// </summary>
        /// <typeparam name="T">Object type to find</typeparam>
        /// <param name="text">Text to search for</param>
        /// <param name="language">Language to search</param>
        /// <param name="textIdAccessor">Field accessor for the localized text ID</param>
        /// <param name="fuzzy">true to search for strings containing the specified text, false for an exact match</param>
        /// <returns>Enumerable of found objects</returns>
        public IEnumerable<T> Search<T>(string text, Language language, Func<T, IEnumerable<uint>> textIdAccessor,
            bool fuzzy = false)
            where T : class, new()
        {
            GameTable<T> gameTable = GetGameTable<T>();
            if (gameTable == null)
                return Enumerable.Empty<T>();
            IEnumerable<uint> ids = Search(text, language, fuzzy);
            return gameTable.Entries.Where(i => textIdAccessor(i)?.Any(x => ids.Contains(x)) ?? false);
        }

        private ITextReverseIndex GetIndex(Language language)
        {
            reverseIndexDictionary.TryGetValue(language, out ITextReverseIndex index);
            return index;
        }
    }
}
