using NexusForever.Game.Static.Combat;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogFallingDamage : ICombatLog
    {
        public CombatLogType Type => CombatLogType.FallingDamage;

        public uint CasterId { get; set; }
        public uint Damage { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CasterId);
            writer.Write(Damage);
        }
    }
}
