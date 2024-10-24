using NexusForever.Game.Static.Combat;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogKillStreak : ICombatLog
    {
        public CombatLogType Type => CombatLogType.KillStreak;

        public uint UnitId { get; set; }
        public byte StatType { get; set; } // 5u
        public uint StreakAmount { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(StatType, 5u);
            writer.Write(StreakAmount);
        }
    }
}
