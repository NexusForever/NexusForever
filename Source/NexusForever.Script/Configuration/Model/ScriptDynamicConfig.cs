using NexusForever.Shared.Configuration;

namespace NexusForever.Script.Configuration.Model
{
    [ConfigurationBind]
    public class ScriptDynamicConfig
    {
        public bool Enable { get; set; } = false;
        public string Directory { get; set; }
    }
}
