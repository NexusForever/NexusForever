using NexusForever.Shared.Configuration;

namespace NexusForever.Game.Configuration.Model
{
    [ConfigurationBind]
    public class ContactConfig
    {
        public uint? MaxFriends { get; set; } = 100u;
        public uint? MaxRivals { get; set; } = 100u;
        public uint? MaxIgnored { get; set; } = 100u;
        public float? MaxRequestDuration { get; set; } = 7f;
    }
}