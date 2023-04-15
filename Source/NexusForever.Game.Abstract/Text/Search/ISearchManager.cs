using NexusForever.Game.Static;

namespace NexusForever.Game.Abstract.Text.Search
{
    public interface ISearchManager
    {
        void Initialise();

        /// <summary>
        /// Search for text ID's
        /// </summary>
        /// <param name="text"></param>
        /// <param name="language"></param>
        /// <param name="fuzzy"></param>
        IEnumerable<uint> Search(string text, Language language, bool fuzzy = false);

        IEnumerable<T> Search<T>(string text, Language language, Func<T, uint> textIdAccessor, bool fuzzy = false) where T : class, new();

        /// <summary>
        /// Find objects matching the specified search string
        /// </summary>
        /// <typeparam name="T">Object type to find</typeparam>
        /// <param name="text">Text to search for</param>
        /// <param name="language">Language to search</param>
        /// <param name="textIdAccessor">Field accessor for the localized text ID</param>
        /// <param name="fuzzy">true to search for strings containing the specified text, false for an exact match</param>
        /// <returns>Enumerable of found objects</returns>
        IEnumerable<T> Search<T>(string text, Language language, Func<T, IEnumerable<uint>> textIdAccessor, bool fuzzy = false) where T : class, new();
    }
}