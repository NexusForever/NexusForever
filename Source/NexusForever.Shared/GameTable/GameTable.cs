using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Configuration;
using NexusForever.Shared.Configuration;

namespace NexusForever.Shared.GameTable
{
    public static class GameTableFactory
    {
        public static GameTable<T> Load<T>(string fileName) where T : class, new()
        {
            var path = SharedConfiguration.Configuration.GetValue<string>("GameTablePath", "tbl");
            return new GameTable<T>(Path.Combine(path, fileName));
        }

        public static TextTable LoadText(string fileName)
        {
            var path = SharedConfiguration.Configuration.GetValue<string>("GameTablePath", "tbl");
            return new TextTable(Path.Combine(path, fileName));
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TextHeader
    {
        public uint Id;
        public uint Offset;
    }
    public class TextEntry
    {
        public TextEntry(Language language, uint id, string text)
        {
            Id = id;
            Language = Language;
            Text = text;
        }
        public uint Id { get; private set; }
        public Language Language { get; private set; }
        public string Text { get; private set; }
    }
    public class TextTable
    {

        public TextEntry[] Entries { get; private set; }

        private readonly TextTableHeader header;
        private class TupleComparer : IComparer<Tuple<uint, int>>
        {
            public int Compare(Tuple<uint, int> x, Tuple<uint, int> y)
            {
                return x.Item1.CompareTo(y.Item1);
            }
        }

        public TextTable(string path)
        {
            using (var stream = File.OpenRead(path))
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
                    while (reader.PeekChar() != '\0')
                    {
                        textBuilder.Append(reader.ReadChar());
                    }

                    final.Add(new TextEntry(header.Language, entry.Id, textBuilder.ToString()));
                }

                Entries = final.ToArray();
                Index = entries.Select((item, index) => Tuple.Create(item.Id, index)).ToList();
                Index.Sort(new TupleComparer());
                #region DEBUG
                Debug.Assert(GetText((uint) 1) == "Cancel");
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
        List<Tuple<uint, int>> Index {get;}

        public string GetText(uint id)
        {
            var index = Index.BinarySearch(Tuple.Create(id, 0), new TupleComparer());
            if(index < 0) return null;
            return Entries[Index[index].Item2].Text;
        }
    }

    public class GameTable<T> where T : class, new()
    {
        public T[] Entries { get; private set; }

        private readonly GameTableHeader header;
        private int[] lookup;
        private static Dictionary<FieldInfo, GameTableFieldArrayAttribute> attributeCache;
        private const int minimumBufferSize = 1024;
        private const int maximumBufferSize = 16 * 1024;
        private const int recordSizeMultiplier = 32; // How many records do we want buffered.
        static GameTable()
        {
            attributeCache = new Dictionary<FieldInfo, GameTableFieldArrayAttribute>();
            foreach (FieldInfo modelField in typeof(T).GetFields())
            {
                GameTableFieldArrayAttribute attribute = modelField.GetCustomAttribute<GameTableFieldArrayAttribute>();
                attributeCache.Add(modelField, attribute);
            }

            headerSize = Marshal.SizeOf<GameTableHeader>();
            tableFieldSize = Marshal.SizeOf<GameTableField>();
            bufferSize = tableFieldSize * recordSizeMultiplier;
            if (bufferSize < minimumBufferSize) bufferSize = minimumBufferSize;
            if (bufferSize > maximumBufferSize) bufferSize = maximumBufferSize;
        }

        private static readonly int headerSize;
        private static readonly int tableFieldSize;
        private static readonly int bufferSize;

        public GameTable(string path)
        {
            using (var stream = new BufferedStream(File.OpenRead(path), bufferSize))
            using (var reader = new BinaryReader(stream))
            {
                // header
                ReadOnlySpan<GameTableHeader> headerSpan = MemoryMarshal.Cast<byte, GameTableHeader>(
                    reader.ReadBytes(headerSize));

                header = headerSpan[0];

                // fields
                stream.Position = headerSize + (int)header.FieldOffset;
                ReadOnlySpan<GameTableField> fields = MemoryMarshal.Cast<byte, GameTableField>(
                    reader.ReadBytes(tableFieldSize * (int)header.FieldCount));


                // Should this be debug only?
                ValidateModelFields(fields, attributeCache);

                ReadEntries(reader, fields, attributeCache);
            }
        }

        private void ValidateModelFields(ReadOnlySpan<GameTableField> fields, Dictionary<FieldInfo, GameTableFieldArrayAttribute> attributeCache)
        {
            FieldInfo[] modelFields = attributeCache.Keys.ToArray();
            if (modelFields.Length != fields.Length)
            {
                // models with arrays will not have the correct number of fields, compensate for this
                uint fieldCount = 0u;
                foreach (FieldInfo modelField in modelFields)
                {
                    GameTableFieldArrayAttribute attribute = attributeCache[modelField];
                    if (attribute != null)
                        fieldCount += attribute.Length;
                    else
                        fieldCount++;
                }

                if (fieldCount != fields.Length)
                    throw new GameTableException($"GameTable model has an invalid field count {modelFields.Length}! Expected: {fields.Length}");
            }

            void ValidateModelField(DataType dataType, Type modelFieldType)
            {
                switch (dataType)
                {
                    case DataType.UInt:
                        if (modelFieldType != typeof(uint))
                            throw new GameTableException($"Invalid GameTable model field type {modelFieldType}! Expected: {typeof(uint)}");
                        break;
                    case DataType.Single:
                        if (modelFieldType != typeof(float))
                            throw new GameTableException($"Invalid GameTable model field type {modelFieldType}! Expected: {typeof(float)}");
                        break;
                    case DataType.Boolean:
                        if (modelFieldType != typeof(bool))
                            throw new GameTableException($"Invalid GameTable model field type {modelFieldType}! Expected: {typeof(bool)}");
                        break;
                    case DataType.ULong:
                        if (modelFieldType != typeof(ulong))
                            throw new GameTableException($"Invalid GameTable model field type {modelFieldType}! Expected: {typeof(ulong)}");
                        break;
                    case DataType.String:
                        if (modelFieldType != typeof(string))
                            throw new GameTableException($"Invalid GameTable model field type {modelFieldType}! Expected: {typeof(string)}");
                        break;
                    default:
                        throw new GameTableException($"Unknown GameTable field type {dataType}!");
                }
            }

            int fieldIndex = 0;
            foreach (FieldInfo modelField in modelFields)
            {
                GameTableFieldArrayAttribute attribute = attributeCache[modelField];
                if (attribute != null)
                {
                    Type elementType = modelField.FieldType.GetElementType();
                    for (uint i = 0u; i < attribute.Length; i++)
                        ValidateModelField(fields[fieldIndex++].Type, elementType);
                }
                else
                    ValidateModelField(fields[fieldIndex++].Type, modelField.FieldType);
            }
        }

        private void ReadEntries(BinaryReader reader, ReadOnlySpan<GameTableField> fields, Dictionary<FieldInfo, GameTableFieldArrayAttribute> attributeCache)
        {
            var idField = attributeCache.Keys.FirstOrDefault(i => string.Equals(i.Name, "id", StringComparison.OrdinalIgnoreCase));
            Entries = new T[header.RecordCount];
            lookup = new int[header.MaxId];
            for (int i = 0; i < (int)header.RecordCount; i++)
            {
                long recordStartPosition = headerSize + (long)header.RecordOffset + (long)header.RecordSize * i;
                if (reader.BaseStream.Position != recordStartPosition)
                    reader.BaseStream.Position = recordStartPosition;

                T entry = new T();

                int fieldIndex = 0;
                FieldInfo[] typeFields = attributeCache.Keys.ToArray();
                foreach (FieldInfo modelField in typeFields)
                {
                    GameTableFieldArrayAttribute attribute = attributeCache[modelField];
                    if (attribute != null)
                    {
                        Array array = Array.CreateInstance(modelField.FieldType.GetElementType(), attribute.Length);
                        for (uint j = 0u; j < attribute.Length; j++)
                        {
                            DataType dataType = fields[fieldIndex++].Type;
                            switch (dataType)
                            {
                                case DataType.UInt:
                                    array.SetValue(reader.ReadUInt32(), j);
                                    break;
                                case DataType.Single:
                                    array.SetValue(reader.ReadSingle(), j);
                                    break;
                                case DataType.Boolean:
                                    array.SetValue(Convert.ToBoolean(reader.ReadUInt32()), j);
                                    break;
                                case DataType.ULong:
                                    array.SetValue(reader.ReadUInt64(), j);
                                    break;
                            }
                        }

                        modelField.SetValue(entry, array);
                    }
                    else
                    {
                        DataType dataType = fields[fieldIndex++].Type;
                        switch (dataType)
                        {
                            case DataType.UInt:
                                modelField.SetValue(entry, reader.ReadUInt32());
                                break;
                            case DataType.Single:
                                modelField.SetValue(entry, reader.ReadSingle());
                                break;
                            case DataType.Boolean:
                                modelField.SetValue(entry, Convert.ToBoolean(reader.ReadUInt32()));
                                break;
                            case DataType.ULong:
                                modelField.SetValue(entry, reader.ReadUInt64());
                                break;
                            case DataType.String:
                                {
                                    uint offset1 = reader.ReadUInt32();
                                    uint offset2 = reader.ReadUInt32();
                                    uint offset3 = Math.Max(offset1, offset2);

                                    long position = reader.BaseStream.Position;
                                    reader.BaseStream.Position =
                                        headerSize + (long)header.RecordOffset + offset3;

                                    modelField.SetValue(entry, reader.ReadWideString());
                                    reader.BaseStream.Position = position;

                                    if (fieldIndex < typeFields.Length - 1)
                                        if (offset1 == 0 && fields[fieldIndex].Type != DataType.String)
                                            reader.BaseStream.Position += 4;
                                    break;
                                }
                        }
                    }
                }

                var id = (uint)idField.GetValue(entry);
                Entries[i] = entry;
                lookup[id] = i;
            }
        }

        public T GetEntry(ulong id)
        {
            if (id >= header.MaxId)
                return null;

            int lookupId = lookup[id];
            if (lookupId == -1)
                return null;

            return Entries[lookupId];
        }
    }
}
