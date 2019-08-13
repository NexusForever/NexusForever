using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;
using NexusForever.Shared.Network;

namespace NexusForever.Shared.GameTable
{
    public class TextTable
    {
        public TextTableEntry[] Entries { get; private set; }

        private readonly TextTableHeader header;
        private int[] lookup;

        [JsonConstructor]
        public TextTable(IEnumerable<TextTableEntry> entries)
        {
            Entries = entries.ToArray();
            BuildLookup();
        }

        public TextTable(string path)
        {
            using (FileStream fileStream = File.OpenRead(path))
            using (var stream = new BufferedStream(fileStream, 64 * 1024))
            using (var reader = new BinaryReader(stream, Encoding.Unicode))
            {
                if (reader.BaseStream.Remaining() < Marshal.SizeOf<TextTableHeader>())
                    throw new InvalidDataException();

                // header
                ReadOnlySpan<TextTableHeader> headerSpan
                    = MemoryMarshal.Cast<byte, TextTableHeader>(reader.ReadBytes(Marshal.SizeOf<TextTableHeader>()));

                header = headerSpan[0];

                if (header.Signature != 0x4C544558)
                    throw new InvalidDataException();

                // fields
                stream.Position = Marshal.SizeOf<TextTableHeader>() + (int)header.RecordOffset;

                ReadOnlySpan<TextTableField> fields = MemoryMarshal.Cast<byte, TextTableField>(
                    reader.ReadBytes(Marshal.SizeOf<TextTableField>() * (int)header.RecordCount));

                ReadEntries(reader, fields);
                BuildLookup();
            }
        }

        private void ReadEntries(BinaryReader reader, ReadOnlySpan<TextTableField> fields)
        {
            Entries = new TextTableEntry[fields.Length];

            // build string table
            reader.BaseStream.Position = Marshal.SizeOf<TextTableHeader>() + (long)header.StringTableOffset;
            using (var stringTable = new StringTable(reader.ReadBytes((int)header.StringTableLength * 2)))
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    TextTableField field = fields[i];
                    string @string = stringTable.GetEntry(field.Offset * 2);
                    Entries[i] = new TextTableEntry(header.Language, field.Id, @string);
                }
            }
        }

        private void BuildLookup()
        {
            // final field will always have the max id
            lookup = new int[Entries[Entries.Length - 1].Id + 1];
            for (int i = 0; i < lookup.Length; i++)
                lookup[i] = -1;

            for (int i = 0; i < Entries.Length; i++)
            {
                TextTableEntry entry = Entries[i];
                lookup[entry.Id] = i;
            }
        }

        public string GetEntry(uint id)
        {
            if (id >= lookup.Length)
                return null;

            int lookupId = lookup[id];
            if (lookupId == -1)
                return null;

            return Entries[lookupId].Text;
        }
    }
}
