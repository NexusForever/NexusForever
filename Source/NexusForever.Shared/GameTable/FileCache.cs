using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Cryptography;

namespace NexusForever.Shared.GameTable
{
    public static class FileCache
    {
        public static T LoadWithCache<T>(string fileName, Func<string, T> creator)
        {
            if (!SharedConfiguration.Configuration.GetValue<bool>("UseCache", true)) return creator(fileName);
            var cacheName = GetCacheFileName(fileName);
            if (File.Exists(cacheName))
            {
                return JObject.Parse(File.ReadAllText(cacheName)).ToObject<T>();
            }
            else
            {
                var obj = creator(fileName);
                File.WriteAllText(cacheName, JObject.FromObject(obj).ToString());
                return obj;
            }
        }

        private static string GetCacheFileName(string fileName)
        {
            var cacheFolder = Directory
                .CreateDirectory(SharedConfiguration.Configuration.GetValue<string>("CachePath", "cache")).FullName;
            string ext = Path.GetExtension(fileName).TrimStart('.');
            ext = ext + ".cache";
            string hash = FileHasher.HashFile(fileName).Substring(0, 7);
            return Path.Combine(cacheFolder, $"{Path.GetFileNameWithoutExtension(fileName)}.{hash}.{ext}");
        }
    }
}