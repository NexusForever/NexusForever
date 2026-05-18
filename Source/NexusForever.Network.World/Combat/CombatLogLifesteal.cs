using NexusForever.Game.Static.Combat;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogLifesteal : ICombatLog
    {
        public CombatLogType Type => CombatLogType.Lifesteal;

        public uint UnitId { get; set; }
        public uint HealthStolen { get; set; }
        public uint Absorption { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(HealthStolen);
            writer.Write(Absorption);
        }
    }
}
