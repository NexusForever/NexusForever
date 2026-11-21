using NexusForever.Game.Static.Combat;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogLAS : ICombatLog
    {
        public CombatLogType Type => CombatLogType.LAS;

        public uint UnitId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
        }
    }
}
