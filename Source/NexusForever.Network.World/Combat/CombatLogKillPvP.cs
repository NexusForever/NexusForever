using NexusForever.Game.Static.Combat;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogKillPvP : ICombatLog
    {
        public CombatLogType Type => CombatLogType.KillPvP;

        public CombatLogCastData CastData { get; set; }

        public void Write(GamePacketWriter writer)
        {
            CastData.Write(writer);
        }
    }
}
