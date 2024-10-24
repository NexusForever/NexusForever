using NexusForever.Game.Static.Combat;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogDeath : ICombatLog
    {
        public CombatLogType Type => CombatLogType.Death;

        public uint UnitId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
        }
    }
}
