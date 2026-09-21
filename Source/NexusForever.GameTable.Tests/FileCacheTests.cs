using System.Reflection;
using Microsoft.Extensions.Configuration;
using NexusForever.GameTable.Configuration.Model;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using Xunit;

namespace NexusForever.GameTable.Tests;

// FileCache and LegacyServiceProvider have process-wide state.
[CollectionDefinition("FileCache", DisableParallelization = true)]
public class FileCacheCollection;

[Collection("FileCache")]
public class FileCacheTests : IDisposable
{
    private static readonly FieldInfo CacheCheck = typeof(FileCache)
        .GetField("cacheCheck", BindingFlags.NonPublic | BindingFlags.Static)!;
    private readonly IServiceProvider previousProvider = LegacyServiceProvider.Provider;
    private readonly string root = Path.Combine(Path.GetTempPath(), "nexus-cache-tests-" + Guid.NewGuid());
    private readonly string cachePath;
    private readonly string sourcePath;
    private static string Version => Convert.ToHexString(typeof(FileCache).Assembly.ManifestModule.ModuleVersionId.ToByteArray());

    public FileCacheTests()
    {
        Directory.CreateDirectory(root);
        cachePath = Path.Combine(root, "cache");
        sourcePath = Path.Combine(root, "source.tbl");
        File.WriteAllText(sourcePath, "source data");
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
        {
            ["Cache:UseCache"] = "true",
            ["Cache:CachePath"] = cachePath
        }).Build();
        var shared = new SharedConfiguration(configuration);
        shared.Initialise<ConfigurationModel>();
        LegacyServiceProvider.Provider = new ConfigurationProvider(shared);
        CacheCheck.SetValue(null, 0);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task FailedCleanupCanBeRetriedAfterFilesystemIsRepaired(bool blockVersionFile)
    {
        if (blockVersionFile)
            Directory.CreateDirectory(Path.Combine(cachePath, "cacheInfo.txt"));
        else
            File.WriteAllText(cachePath, "This file prevents creating the cache directory.");

        int calls = 0;
        string Create(string path)
        {
            Interlocked.Increment(ref calls);
            return File.ReadAllText(path);
        }

        Exception error = Record.Exception(() => FileCache.LoadWithCache(sourcePath, Create));
        Assert.True(error is IOException or UnauthorizedAccessException, $"Unexpected error: {error}");
        Assert.Equal(0, calls);

        if (blockVersionFile)
            Directory.Delete(Path.Combine(cachePath, "cacheInfo.txt"));
        else
            File.Delete(cachePath);

        Task<string> retry = Task.Run(() => FileCache.LoadWithCache(sourcePath, Create));
        try
        {
            Assert.Equal("source data", await retry.WaitAsync(TimeSpan.FromSeconds(3)));
            Assert.Equal(1, calls);
            Assert.Equal(Version, File.ReadAllText(Path.Combine(cachePath, "cacheInfo.txt")));
            Assert.Equal("source data", FileCache.LoadWithCache<string>(sourcePath,
                _ => throw new InvalidOperationException("Expected a cache hit.")));
        }
        finally
        {
            // Release the original implementation's stuck loop after a failed assertion,
            // so the regression test does not leave a worker running or hang the suite.
            if (!retry.IsCompleted)
            {
                CacheCheck.SetValue(null, 0);
                await retry.WaitAsync(TimeSpan.FromSeconds(10));
            }
        }
    }

    [Fact]
    public void OutdatedCacheIsRemovedBeforeCreatingANewEntry()
    {
        Directory.CreateDirectory(cachePath);
        File.WriteAllText(Path.Combine(cachePath, "cacheInfo.txt"), "old version");
        string stale = Path.Combine(cachePath, "stale.cache");
        File.WriteAllText(stale, "old data");

        Assert.Equal("source data", FileCache.LoadWithCache(sourcePath, File.ReadAllText));
        Assert.False(File.Exists(stale));
        Assert.Equal(Version, File.ReadAllText(Path.Combine(cachePath, "cacheInfo.txt")));
    }

    [Fact]
    public void MatchingVersionPreservesExistingFiles()
    {
        Directory.CreateDirectory(cachePath);
        File.WriteAllText(Path.Combine(cachePath, "cacheInfo.txt"), Version);
        string existing = Path.Combine(cachePath, "existing.cache");
        File.WriteAllText(existing, "keep me");

        Assert.Equal("source data", FileCache.LoadWithCache(sourcePath, File.ReadAllText));
        Assert.Equal("keep me", File.ReadAllText(existing));
    }

    public void Dispose()
    {
        CacheCheck.SetValue(null, 0);
        LegacyServiceProvider.Provider = previousProvider;
        Directory.Delete(root, recursive: true);
    }

    public class ConfigurationModel
    {
        public CacheConfig Cache { get; set; } = new();
    }

    private sealed class ConfigurationProvider(SharedConfiguration configuration) : IServiceProvider
    {
        public object GetService(Type serviceType) =>
            serviceType == typeof(SharedConfiguration) ? configuration : null;
    }
}
