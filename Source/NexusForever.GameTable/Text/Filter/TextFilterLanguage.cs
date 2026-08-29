using System.Collections.Immutable;
using NexusForever.Game.Static;
using NexusForever.GameTable.Model;
using NexusForever.GameTable.Text.Static;

namespace NexusForever.GameTable.Text.Filter
{
    public class TextFilterLanguage : ITextFilterLanguage
    {
        public Language Language { get; }

        private readonly ImmutableDictionary<TextFilterClass, ImmutableList<string>> words;

        /// <summary>
        /// Create a new <see cref="ITextFilterLanguage"/> class with the specified <see cref="Static.Language"/> and collection of <see cref="WordFilterEntry"/>'s.
        /// </summary>
        public TextFilterLanguage(Language language, List<WordFilterEntry> entries)
        {
            Language = language;

            words = entries
                .Select(e => e.UserTextFilterClassEnum)
                .Distinct()
                .ToImmutableDictionary(k => (TextFilterClass)k, k => entries
                    .Where(e => e.UserTextFilterClassEnum >= k)
                    .Select(e => e.Filter)
                    .ToImmutableList());
        }

        /// <summary>
        /// Returns if supplied string meets <see cref="TextFilterClass"/> filtering.
        /// </summary>
        public bool IsTextValid(string text, TextFilterClass filterClass)
        {
            if (!words.TryGetValue(filterClass, out ImmutableList<string> filerWords))
                return false;

            return !filerWords.Any(w => text.Contains(w, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
