using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NexusForever.Shared.GameTable
{
    public class GameTable<T> where T : class, new()
    {
        public T[] Entries { get; private set; }

        private readonly GameTableHeader header;
        private int[] lookup;
        private static Dictionary<FieldInfo, GameTableFieldArrayAttribute> attributeCache;

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
            bufferSize = tableFieldSize * 16;
            if (bufferSize < 1024) bufferSize = 1024;
            if (bufferSize > 16 * 1024) bufferSize = 16 * 1024;
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
                reader.BaseStream.Position = headerSize + (long)header.RecordOffset + (long)header.RecordSize * i;

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

                var id = (uint) idField.GetValue(entry);
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
