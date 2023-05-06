using NexusForever.Shared.Configuration;

namespace NexusForever.Script.Configuration.Model
{
    [ConfigurationBind]
    public class ScriptConfig
    {
        public bool Enable { get; set; } = false;
        public string Directory { get; set; }
        public ScriptDynamicConfig Dynamic { get; set; }
    }
}
