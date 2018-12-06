using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using NexusForever.Shared.Configuration;

namespace NexusForever.Shared.GameTable
{
    public static class GameTableFactory
    {
        public static object LoadGameTable(Type type, string fileName)
        {
            string path = SharedConfiguration.Configuration.GetValue("GameTablePath", "tbl");
            string filePath = Path.Combine(path, fileName);

            Type table = typeof(GameTable<>).MakeGenericType(type);
            return Activator.CreateInstance(table, filePath);
        }

        public static TextTable LoadTextTable(string fileName)
        {
            string path = SharedConfiguration.Configuration.GetValue("GameTablePath", "tbl");
            string filePath = Path.Combine(path, fileName);

            if (!File.Exists(filePath))
                return null;

            return FileCache.LoadWithCache(filePath, file => new TextTable(file));
        }
    }
}