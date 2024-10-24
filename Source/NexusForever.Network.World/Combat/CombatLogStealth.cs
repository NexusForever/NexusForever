using NexusForever.Game.Static.Combat;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogStealth : ICombatLog
    {
        public CombatLogType Type => CombatLogType.Stealth;

        public uint UnitId { get; set; }
        public bool BExiting { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(BExiting);
        }
    }
}
