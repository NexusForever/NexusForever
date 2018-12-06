using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Cryptography;

namespace NexusForever.Shared.GameTable
{
    public static class FileCache
    {
        public static T LoadWithCache<T>(string fileName, Func<string, T> creator)
        {
            if (!SharedConfiguration.Configuration.GetValue("UseCache", true))
                return creator(fileName);

            string cacheName = GetCacheFileName(fileName);
            if (File.Exists(cacheName))
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(cacheName));

            T obj = creator(fileName);
            File.WriteAllText(cacheName, JsonConvert.SerializeObject(obj));
            return obj;
        }

        private static string GetCacheFileName(string fileName)
        {
            string cacheFolder = Directory
                .CreateDirectory(SharedConfiguration.Configuration.GetValue("CachePath", "cache")).FullName;

            string ext = Path.GetExtension(fileName).TrimStart('.');
            string hash = FileHasher.HashFile(fileName).Substring(0, 7);
            return Path.Combine(cacheFolder, $"{Path.GetFileNameWithoutExtension(fileName)}.{hash}.{ext}.cache");
        }
    }
}
