using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Static;
using NexusForever.WorldServer.Game.TextSearch;
using NLog;

namespace NexusForever.WorldServer.Game
{
    /// <summary>
    ///     Responsible for looking up text and objects that the text references.
    /// </summary>
    public static class SearchManager
    {
        private static ImmutableDictionary<Language, TextReverseIndex> reverseIndexDictionary;
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public static void Initialise()
        {
            log.Info("Creating reverse text lookups.");
            Dictionary<Language, TextReverseIndex> index = new Dictionary<Language, TextReverseIndex>
            {
                [Language.English] = new TextReverseIndex(GameTableManager.TextEnglish),
                [Language.French] = new TextReverseIndex(GameTableManager.TextFrench),
                [Language.German] = new TextReverseIndex(GameTableManager.TextGerman)
            };
            reverseIndexDictionary = index.ToImmutableDictionary();
            foreach (KeyValuePair<Language, TextReverseIndex> kvp in index)
            {
                if (kvp.Value.IsEmpty)
                    log.Warn($"Language {kvp.Key} was not loaded, and will not be used for text search");
                else
                    log.Debug($"Language {kvp.Key} loaded.");
            }
        }


        private static GameTable<T> GetGameTable<T>() where T : class, new()
        {
            return typeof(GameTableManager).GetProperties(BindingFlags.Static | BindingFlags.Public)
                .Where(i => i.PropertyType == typeof(GameTable<T>))
                .Select(property => property.GetValue(null) as GameTable<T>)
                .FirstOrDefault();
        }

        /// <summary>
        /// Search for text ID's
        /// </summary>
        /// <param name="text"></param>
        /// <param name="language"></param>
        /// <param name="fuzzy"></param>
        /// <returns></returns>
        public static IEnumerable<uint> Search(string text, Language language, bool fuzzy = false)
        {
            TextReverseIndex index = GetIndex(language);
            if (index == null) 
                return Enumerable.Empty<uint>();
            List<uint> ids = new List<uint>();
            if (fuzzy)
                ids.AddRange(index.FuzzySearch(text));
            else
            {
                uint? id = index.GetId(text);
                if (id != null)
                    ids.Add(id.Value);
            }

            return ids;
        }

        /// <summary>
        ///     Find objects matching the specified search string
        /// </summary>
        /// <typeparam name="T">Object type to find</typeparam>
        /// <param name="text">Text to search for</param>
        /// <param name="language">Language to search</param>
        /// <param name="textIdAccessor">Field accessor for the localized text ID</param>
        /// <param name="fuzzy">true to search for strings containing the specified text, false for an exact match</param>
        /// <returns>Enumerable of found objects</returns>
        public static IEnumerable<T> Search<T>(string text, Language language, Func<T, uint> textIdAccessor,
            bool fuzzy = false)
            where T : class, new()
        {
            GameTable<T> gameTable = GetGameTable<T>();
            if (gameTable == null) 
                return Enumerable.Empty<T>();
            IEnumerable<uint> ids = Search(text, language, fuzzy);
            return gameTable.Entries.Where(i => ids.Contains(textIdAccessor(i)));
        }

        private static TextReverseIndex GetIndex(Language language)
        {
            reverseIndexDictionary.TryGetValue(language, out TextReverseIndex index);
            return index;
        }
    }
}
