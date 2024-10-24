using NexusForever.Game.Static.Combat;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogInterrupted : ICombatLog
    {
        public CombatLogType Type => CombatLogType.Interrupted;

        public uint InterruptingSpellId { get; set; } // 18u
        public ushort Reason { get; set; } // 9u
        public CombatLogCastData CastData { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(InterruptingSpellId, 18u);
            writer.Write(Reason, 9u);
            CastData.Write(writer);
        }
    }
}
