using NexusForever.Game.Static.Combat;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogDurabilityLoss : ICombatLog
    {
        public CombatLogType Type => CombatLogType.DurabilityLoss;

        public uint UnitId { get; set; }
        public float Amount { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Amount);
        }
    }
}
