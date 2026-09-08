using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using NexusForever.GameTable;
using NexusForever.IO.Map;

namespace NexusForever.MapGenerator
{
    internal static class AssetPreparation
    {
        private const string InventoryFile = ".maps.json";

        private static readonly string[] textFiles = typeof(NexusForever.GameTable.GameTableManager).GetProperties()
            .Where(property => property.PropertyType == typeof(TextTable))
            .Select(property => property.GetCustomAttribute<GameDataAttribute>()?.FileName)
            .Where(name => name != null).ToArray();

        public static void Run(Parameters parameters, Action<Parameters> generate)
        {
            if (string.IsNullOrWhiteSpace(parameters.OutputDir))
                throw new ArgumentException("Set an asset output directory with --output.");
            if (parameters.WorldId.HasValue || parameters.GridX.HasValue || parameters.GridY.HasValue)
                throw new ArgumentException("--prepare requires a complete extraction; omit world and grid filters.");

            Directory.CreateDirectory(parameters.OutputDir);
            using var fileLock = LockDirectory(parameters.OutputDir);
            string marker = Path.Combine(parameters.OutputDir, ".extracting");
            if (!File.Exists(marker))
            {
                try
                {
                    Validate(parameters.OutputDir);
                    Console.WriteLine("Using existing maps and tables.");
                    return;
                }
                catch (Exception exception) when (exception is IOException or InvalidDataException or JsonException or TargetInvocationException)
                {
                    Console.WriteLine($"Assets need extraction: {exception.GetBaseException().Message}");
                }
            }

            string patch = parameters.PatchPath;
            if (Directory.Exists(Path.Combine(patch ?? "", "Patch")))
                patch = Path.Combine(patch, "Patch");
            if (!File.Exists(Path.Combine(patch ?? "", "ClientData.index")))
                throw new FileNotFoundException("Set ClientPath to a WildStar installation or its Patch directory.");

            parameters.PatchPath = patch;
            parameters.Extract = true;
            parameters.Generate = true;
            File.WriteAllText(marker, "Extraction is incomplete. Run asset preparation again.\n");
            string destination = parameters.OutputDir;
            string staging = Path.GetFullPath(Path.Combine(destination, ".prepare"));
            try
            {
                if (Directory.Exists(staging))
                    Directory.Delete(staging, recursive: true);
                Directory.CreateDirectory(staging);
                parameters.OutputDir = staging;
                generate(parameters);
                Validate(staging, full: true, checkInventory: false);

                var inventory = Directory.GetFiles(Path.Combine(staging, "map"), "*.nfmap")
                    .ToDictionary(Path.GetFileName, path => new FileInfo(path).Length, StringComparer.Ordinal);
                File.WriteAllText(Path.Combine(staging, InventoryFile), JsonSerializer.Serialize(inventory));

                string maps = Path.Combine(destination, "map");
                Directory.CreateDirectory(maps);
                foreach (string path in Directory.GetFiles(maps, "*.nfmap"))
                    if (!inventory.ContainsKey(Path.GetFileName(path)))
                        File.Delete(path);

                string tables = Path.Combine(destination, "tbl");
                Directory.CreateDirectory(tables);
                foreach (string name in textFiles)
                    if (!File.Exists(Path.Combine(staging, "tbl", name)))
                        File.Delete(Path.Combine(tables, name));

                foreach (string folder in new[] { "map", "tbl" })
                {
                    Directory.CreateDirectory(Path.Combine(destination, folder));
                    foreach (string path in Directory.GetFiles(Path.Combine(staging, folder)))
                        File.Move(path, Path.Combine(destination, folder, Path.GetFileName(path)), overwrite: true);
                }
                File.Move(Path.Combine(staging, InventoryFile), Path.Combine(destination, InventoryFile), overwrite: true);
            }
            finally
            {
                parameters.OutputDir = destination;
                if (Directory.Exists(staging))
                    Directory.Delete(staging, recursive: true);
            }
            File.Delete(marker);
        }

        private static FileStream LockDirectory(string directory)
        {
            try
            {
                return File.Open(Path.Combine(directory, ".prepare.lock"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException exception)
            {
                throw new IOException($"Cannot lock asset directory {directory}; another setup may be preparing it.", exception);
            }
        }

        private static void Validate(string directory, bool full = false, bool checkInventory = true)
        {
            string[] maps = Directory.GetFiles(Path.Combine(directory, "map"), "*.nfmap");
            if (maps.Length == 0)
                throw new InvalidDataException("No map files found.");
            if (checkInventory)
            {
                string inventoryPath = Path.Combine(directory, InventoryFile);
                if (!File.Exists(inventoryPath))
                    throw new InvalidDataException("No map inventory found. Regenerate these assets once with --prepare and a client path.");
                var inventory = JsonSerializer.Deserialize<Dictionary<string, long>>(File.ReadAllText(inventoryPath));
                if (inventory == null || !maps.Select(Path.GetFileName).ToHashSet(StringComparer.Ordinal).SetEquals(inventory.Keys))
                    throw new InvalidDataException("Map files do not match the generated inventory.");
                foreach (string path in maps)
                    if (new FileInfo(path).Length != inventory[Path.GetFileName(path)])
                        throw new InvalidDataException($"Map file size has changed: {path}");
            }
            foreach (string path in maps)
            {
                using var reader = new BinaryReader(File.OpenRead(path));
                try
                {
                    var map = new MapFile();
                    if (full)
                        map.Read(reader);
                    else
                        map.ReadHeader(reader);
                }
                catch (Exception exception) when (exception is IOException or InvalidDataException)
                {
                    throw new InvalidDataException($"{path}: {exception.Message}", exception);
                }
            }

            foreach (var property in typeof(NexusForever.GameTable.GameTableManager).GetProperties())
            {
                var attribute = property.GetCustomAttribute<GameDataAttribute>();
                if (attribute == null || property.PropertyType == typeof(TextTable))
                    continue;
                string path = Path.Combine(directory, "tbl", attribute.FileName ?? $"{property.Name}.tbl");
                if (!File.Exists(path))
                    throw new FileNotFoundException($"Missing required table: {path}");
                ValidateTableHeader(path);
                if (full)
                    Activator.CreateInstance(property.PropertyType, path);
            }

            foreach (string name in textFiles)
            {
                string path = Path.Combine(directory, "tbl", name);
                if (!File.Exists(path))
                {
                    if (name == "en-US.bin")
                        throw new FileNotFoundException($"Missing English text table: {path}");
                    continue;
                }
                ValidateTableHeader(path, text: true);
                if (full)
                    _ = new TextTable(path);
            }
        }

        private static void ValidateTableHeader(string path, bool text = false)
        {
            using var stream = File.OpenRead(path);
            try
            {
                if (text)
                {
                    var header = ReadHeader<TextTableHeader>(stream);
                    if (header.Signature != 0x4C544558 || header.RecordCount == 0)
                        throw new InvalidDataException("Invalid text table header.");
                    RequireRange(stream, header.RecordOffset, header.RecordCount, (ulong)Marshal.SizeOf<TextTableField>());
                    RequireRange(stream, header.StringTableOffset, header.StringTableLength, 2);
                }
                else
                {
                    var header = ReadHeader<GameTableHeader>(stream);
                    if (header.Signature != 0x4454424C || (header.RecordCount > 0
                        && (header.RecordSize == 0 || header.RecordCount > header.TotalRecordSize / header.RecordSize)))
                        throw new InvalidDataException("Invalid game table header.");
                    RequireRange(stream, header.FieldOffset, header.FieldCount, (ulong)Marshal.SizeOf<GameTableField>());
                    RequireRange(stream, header.RecordOffset, header.TotalRecordSize, 1);
                }
            }
            catch (Exception exception) when (exception is IOException or InvalidDataException)
            {
                throw new InvalidDataException($"{path}: {exception.Message}", exception);
            }
        }

        private static T ReadHeader<T>(Stream stream) where T : unmanaged
        {
            Span<byte> bytes = stackalloc byte[Marshal.SizeOf<T>()];
            stream.ReadExactly(bytes);
            return MemoryMarshal.Read<T>(bytes);
        }

        private static void RequireRange(Stream stream, ulong offset, ulong count, ulong size)
        {
            ulong available = (ulong)(stream.Length - stream.Position);
            if (offset > available || count > (available - offset) / size)
                throw new InvalidDataException("Table data extends beyond the end of the file.");
        }
    }
}
