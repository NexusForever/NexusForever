using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Cryptography;
using NLog;

namespace NexusForever.Shared.GameTable
{
    public static class FileCache
    {
        private static Lazy<string> lazyModuleVersion = new(CreateModuleVersionString, LazyThreadSafetyMode.ExecutionAndPublication);
        private static Lazy<DirectoryInfo> lazyCacheDirectory => new(CreateCacheDirectory, LazyThreadSafetyMode.ExecutionAndPublication);
        private static ILogger log = LogManager.GetCurrentClassLogger();
        private static DirectoryInfo CreateCacheDirectory()
        {
            return Directory
                .CreateDirectory(SharedConfiguration.Configuration.GetValue("CachePath", "cache"));
        }

        private static int cacheCheck = 0;
        private static string CreateModuleVersionString()
        {
            return typeof(FileCache).Assembly.ManifestModule.ModuleVersionId.ToByteArray().ToHexString();
        }

        private static void CheckAndCleanupCache()
        {
            int state = Interlocked.CompareExchange(ref cacheCheck, 1, 0);
            
            while (state == 1)
            {
                state = Interlocked.CompareExchange(ref cacheCheck, 1, 0);
                Thread.Sleep(100);
            }
            if (state == 2)
                return;
            DirectoryInfo cacheDirectory = lazyCacheDirectory.Value;
            FileInfo cacheInfoFile = cacheDirectory.EnumerateFiles("cacheInfo.txt").FirstOrDefault();
            if (cacheInfoFile != null && cacheInfoFile.Exists)
            {
                string cacheInfo = File.ReadAllText(cacheInfoFile.FullName);
                if (cacheInfo == lazyModuleVersion.Value)
                {
                    Interlocked.Exchange(ref cacheCheck, 2);
                    return;
                }
            }

            log.Info("Cache files are out of date, removing them.");
            FileInfo[] allFiles = cacheDirectory.GetFiles();

            foreach (FileInfo file in allFiles)
            {
                try
                {
                    log.Debug($"Deleting cache file {file.Name}");
                    file.Delete();
                }
                catch
                {
                    // Ignored.
                }
            }

            File.WriteAllText(Path.Combine(cacheDirectory.FullName, "cacheInfo.txt"), lazyModuleVersion.Value);
            Interlocked.Exchange(ref cacheCheck, 2);
        }

        public static T LoadWithCache<T>(string fileName, Func<string, T> creator)
        {
            if (!SharedConfiguration.Configuration.GetValue("UseCache", true))
                return creator(fileName);
            CheckAndCleanupCache();
            string cacheName = GetCacheFileName(fileName);
            if (File.Exists(cacheName))
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(cacheName));

            T obj = creator(fileName);
            File.WriteAllText(cacheName, JsonConvert.SerializeObject(obj));
            return obj;
        }

        private static string GetCacheFileName(string fileName)
        {
            string cacheFolder = lazyCacheDirectory.Value.FullName;

            string ext = Path.GetExtension(fileName).TrimStart('.');
            string fileHash = Hasher.HashFile(fileName);

            string versionId = lazyModuleVersion.Value;
            string hash = $"{fileHash}-{versionId}".Hash().Substring(0, 7);
            return Path.Combine(cacheFolder, $"{Path.GetFileNameWithoutExtension(fileName)}.{hash}.{versionId}.{ext}.cache");
        }
    }
}
