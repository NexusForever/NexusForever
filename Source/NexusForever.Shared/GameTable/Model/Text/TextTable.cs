using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;

namespace NexusForever.Shared.GameTable.Model.Text
{
    public class TextTable
    {
        public static readonly TextTable Empty = new TextTable();
        public TextEntry[] Entries { get; private set; }

        private readonly TextTableHeader header;
        private class TupleComparer : IComparer<Tuple<uint, int>>
        {
            public int Compare(Tuple<uint, int> x, Tuple<uint, int> y)
            {
                return x.Item1.CompareTo(y.Item1);
            }
        }

        private TextTable()
        {
            Entries = new TextEntry[0];
            Index = new List<Tuple<uint, int>>();
        }

        [JsonConstructor]
        public TextTable(IEnumerable<TextEntry> entries)
        {
            Entries = entries.ToArray();
            Index = new List<Tuple<uint, int>>();
            BuildIndex();
        }

        public TextTable(string path)
        {
            using (var stream = new BufferedStream(File.OpenRead(path), 64 * 1024))
            using (var reader = new BinaryReader(stream, Encoding.Unicode))
            {
                // header
                ReadOnlySpan<TextTableHeader> headerSpan = MemoryMarshal.Cast<byte, TextTableHeader>(
                    reader.ReadBytes(Marshal.SizeOf<TextTableHeader>()));

                header = headerSpan[0];
                var entryStart = (ulong)Marshal.SizeOf<TextTableHeader>() + header.RecordOffset;
                stream.Position = (long)entryStart;
                // fields
                //stream.Position = Marshal.SizeOf<GameTableHeader>() + (int)header.FieldOffset;
                //ReadOnlySpan<GameTableField> fields = MemoryMarshal.Cast<byte, GameTableField>(
                //    reader.ReadBytes(Marshal.SizeOf<GameTableField>() * (int)header.FieldCount));

                // optimisation to prevent too much CPU time being spent on GetCustomAttribute for large tables
                var entrySize = Marshal.SizeOf<TextHeader>();
                List<TextHeader> entries = new List<TextHeader>();
                for (ulong x = 0; x < header.RecordCount; x++)
                {
                    ReadOnlySpan<TextHeader> textHeader = MemoryMarshal.Cast<byte, TextHeader>(reader.ReadBytes(entrySize));
                    entries.Add(textHeader[0]);
                }

                List<TextEntry> final = new List<TextEntry>();
                foreach (TextHeader entry in entries)
                {
                    stream.Position = (long)((ulong)Marshal.SizeOf<TextTableHeader>() + header.NameOffset + (entry.Offset * 2));
                    StringBuilder textBuilder = new StringBuilder();
                    char next;
                    while ((next = reader.ReadChar()) != '\0')
                    {
                        textBuilder.Append(next);
                    }

                    final.Add(new TextEntry(header.Language, entry.Id, textBuilder.ToString()));
                }

                Entries = final.ToArray();
                Index = new List<Tuple<uint, int>>();
                BuildIndex();
                #region DEBUG
                Debug.Assert(GetText((uint)1) == "Cancel");
                #endregion
                //var attributeCache = new Dictionary<FieldInfo, GameTableFieldArrayAttribute>();

                //foreach (FieldInfo modelField in typeof(T).GetFields())
                //{
                //    GameTableFieldArrayAttribute attribute = modelField.GetCustomAttribute<GameTableFieldArrayAttribute>();
                //    attributeCache.Add(modelField, attribute);
                //}

                //ValidateModelFields(fields, attributeCache);

                //ReadEntries(reader, fields, attributeCache);
                //ReadLookupTable(reader);
            }

        }

        private void BuildIndex()
        {
            Index.AddRange(Entries.Select((item, index) => Tuple.Create<uint, int>(item.Id, index)).ToList());
            Index.Sort(new TupleComparer());
        }

        List<Tuple<uint, int>> Index { get; }

        public string GetText(uint id)
        {
            var index = Index.BinarySearch(Tuple.Create(id, 0), new TupleComparer());
            if (index < 0) return null;
            return Entries[Index[index].Item2].Text;
        }
    }
}