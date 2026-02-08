using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Group.Shared
{
    public class GroupCharacter
    {
        public string Name { get; set; }
        public string RealmName { get; set; }
        public Faction Faction { get; set; }
        public Race Race { get; set; }
        public Class Class { get; set; }
        public Sex Sex { get; set; }
        public byte Level { get; set; }
        public byte EffectiveLevel { get; set; }
        public Game.Static.Entity.Path Path { get; set; }
        public float Health { get; set; }
        public float HealthMax { get; set; }
        public float Shield { get; set; }
        public float ShieldMax { get; set; }
        public float InterruptArmour { get; set; }
        public float InterruptArmourMax { get; set; }
        public float Absorption { get; set; }
        public float AbsorptionMax { get; set; }
        public float Focus { get; set; }
        public float FocusMax { get; set; }
        public float HealingAbsorb { get; set; }
        public float HealingAbsorbMax { get; set; }
        public ushort RealmId { get; set; }
        public uint WorldZoneId { get; set; }
        public uint WorldId { get; set; }
        public Position Position { get; set; }
        public uint PhaseFlags1 { get; set; }
        public uint PhaseFlags2 { get; set; }
    }
}
