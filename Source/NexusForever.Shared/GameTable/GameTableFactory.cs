using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.GameTable.Model.Text;

namespace NexusForever.Shared.GameTable
{
    public static class GameTableFactory
    {
        public static object Load(Type type, string fileName)
        {
            MethodInfo method = typeof(GameTableFactory).GetMethods().Single(i =>
                i.GetParameters().Length == 1 && i.Name == nameof(Load) && i.GetGenericArguments().Length == 1);
            return method.MakeGenericMethod(type).Invoke(null, new object[] { fileName });
        }
        public static GameTable<T> Load<T>(string fileName) where T : class, new()
        {
            var path = SharedConfiguration.Configuration.GetValue<string>("GameTablePath", "tbl");
            string filePath = Path.Combine(path, fileName);
            return new GameTable<T>(filePath);
            //return FileCache.LoadWithCache(filePath, file => new GameTable<T>(file));
        }

        public static TextTable LoadText(string fileName)
        {
            var path = SharedConfiguration.Configuration.GetValue<string>("GameTablePath", "tbl");
            string filePath = Path.Combine(path, fileName);
            if (!File.Exists(filePath)) return TextTable.Empty;
            return FileCache.LoadWithCache<TextTable>(filePath, file => new TextTable(file));
        }


    }
}