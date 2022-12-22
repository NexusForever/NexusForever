using NexusForever.Shared.Configuration;

namespace NexusForever.GameTable.Configuration.Model;

[ConfigurationBind]
public class CacheConfig
{
    public bool UseCache { get; set; } = true;
    public string CachePath { get; set; } = "cache";
}
