using NexusForever.Game.Static.Combat;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogResurrect : ICombatLog
    {
        public CombatLogType Type => CombatLogType.Resurrect;

        public uint UnitId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
        }
    }
}
