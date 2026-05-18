using NexusForever.Game.Static.RBAC;
using NexusForever.Shared.Configuration;

namespace NexusForever.Game.Configuration.Model
{
    [ConfigurationBind]
    public class RealmConfig
    {
        public MapConfig Map { get; set; }
        public ushort RealmId { get; set; }
        public string MessageOfTheDay { get; set; }
        public uint LengthOfInGameDay { get; set; }
        public bool CrossFactionChat { get; set; } = true;
        public uint MaxPlayers { get; set; } = 50u;
        public Role? DefaultRole { get; set; } = Role.Player;
    }
}