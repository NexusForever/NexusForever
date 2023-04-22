using NexusForever.Shared.Configuration;

namespace NexusForever.GameTable.Configuration.Model;

[ConfigurationBind]
public class GameTableConfig
{
    public string GameTablePath { get; set; } = "tbl";
    public CacheConfig Cache { get; set; }
}
