using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NexusForever.Shared.GameTable;

namespace NexusForever.WorldServer.Game.TextSearch
{
    public class TextReverseIndex
    {
        private readonly ImmutableDictionary<string, uint> index;
        public bool IsEmpty => !index.Keys.Any();

        public TextReverseIndex(TextTable textTable)
        {
            if (textTable == null)
            {
                this.index = ImmutableDictionary<string, uint>.Empty;
                return;
            }

            Dictionary<string, uint> index = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);

            foreach (TextTableEntry entry in textTable.Entries)
                index[string.Intern(entry.Text)] = entry.Id;

            this.index = index.ToImmutableDictionary();
        }

        public uint? GetId(string text)
        {
            if (!index.TryGetValue(text, out uint val)) 
                return null;
            return val;
        }

        public IEnumerable<uint> FuzzySearch(string text)
        {
            return index.Keys.Where(i => i.Contains(text, StringComparison.OrdinalIgnoreCase)).Select(i => index[i])
                .ToList();
        }
    }
}
