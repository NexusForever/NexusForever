using NexusForever.Game.Static.Combat;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Spell;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogTransference : ICombatLog
    {
        public class CombatHealData : IWritable
        {
            public uint HealedUnitId { get; set; }
            public uint HealAmount { get; set; }
            public Vital Vital { get; set; }
            public uint Overheal { get; set; }
            public uint Absorption { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(HealedUnitId);
                writer.Write(HealAmount);
                writer.Write(Vital, 5u);
                writer.Write(Overheal);
                writer.Write(Absorption);
            }
        }
        
        
        public CombatLogType Type => CombatLogType.Transference;

        public uint DamageAmount { get; set; }
        public DamageType DamageType { get; set; } // 3u
        public uint Shield { get; set; }
        public uint Absorption { get; set; }
        public uint Overkill { get; set; }
        public uint GlanceAmount { get; set; }
        public bool BTargetVulnerable { get; set; }
        public List<CombatHealData> HealedUnits { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(DamageAmount);
            writer.Write(DamageType, 3u);
            writer.Write(Shield);
            writer.Write(Absorption);
            writer.Write(Overkill);
            writer.Write(GlanceAmount);
            writer.Write(BTargetVulnerable);
            writer.Write(HealedUnits.Count);

            HealedUnits.ForEach(unit => unit.Write(writer));
        }
    }
}
