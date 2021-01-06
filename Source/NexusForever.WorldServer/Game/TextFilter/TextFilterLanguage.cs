using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.GameTable.Static;
using NexusForever.WorldServer.Game.TextFilter.Static;

namespace NexusForever.WorldServer.Game.TextFilter
{
    public class TextFilterLanguage
    {
        public Language Language { get; }

        private readonly ImmutableDictionary<TextFilterClass, ImmutableList<string>> words;

        /// <summary>
        /// Create a new <see cref="TextFilterLanguage"/> class with the specified <see cref="Shared.GameTable.Static.Language"/> and collection of <see cref="WordFilterEntry"/>'s.
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

            return !filerWords.Any(w => text.Equals(w, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
