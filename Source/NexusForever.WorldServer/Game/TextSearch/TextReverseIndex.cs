using System;
using System.Collections.Generic;
using System.Linq;
using NexusForever.Shared.GameTable;

namespace NexusForever.WorldServer.Game.TextSearch
{
    public class TextReverseIndex
    {
        private readonly Dictionary<string, List<uint>> index;
        public bool IsEmpty => !index.Keys.Any();

        public TextReverseIndex(TextTable textTable)
        {
            index = new Dictionary<string, List<uint>>(StringComparer.OrdinalIgnoreCase);
            if (textTable == null)
                return;

            foreach (TextTableEntry entry in textTable.Entries)
                AddEntry(index, entry);
        }

        private void AddEntry(Dictionary<string, List<uint>> dictionary, TextTableEntry entry)
        {
            if (!dictionary.TryGetValue(entry.Text, out List<uint> list) || list == null)
                dictionary[entry.Text] = list = new List<uint>();
            list.Add(entry.Id);
        }

        public IEnumerable<uint> ExactSearch(string text)
        {
            if (IsEmpty)
                return null;
            if (!index.TryGetValue(text, out List<uint> values))
                return Enumerable.Empty<uint>();
            return values ?? Enumerable.Empty<uint>();
        }

        public IEnumerable<uint> FuzzySearch(string text)
        {
            return index.Keys.Where(i => i.Contains(text, StringComparison.OrdinalIgnoreCase)).SelectMany(i => index[i]).Distinct()
                .ToList();
        }
    }
}
