using NexusForever.Game.Static.Combat;
using NexusForever.Game.Static.Spell;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogTransference : ICombatLog
    {
        public CombatLogType Type => CombatLogType.Transference;

        public uint DamageAmount { get; set; }
        public DamageType DamageType { get; set; } // 3u
        public uint Shield { get; set; }
        public uint Absorption { get; set; }
        public uint Overkill { get; set; }
        public uint GlanceAmount { get; set; }
        public bool BTargetVulnerable { get; set; }
        public List<CombatLogCastData> CastDatas { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(DamageAmount);
            writer.Write(DamageType, 3u);
            writer.Write(Shield);
            writer.Write(Absorption);
            writer.Write(Overkill);
            writer.Write(GlanceAmount);
            writer.Write(BTargetVulnerable);
            writer.Write(CastDatas.Count);

            foreach (CombatLogCastData castData in CastDatas)
                castData.Write(writer);
        }
    }
}
