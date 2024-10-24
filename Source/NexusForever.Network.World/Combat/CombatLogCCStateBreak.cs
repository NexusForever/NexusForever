using NexusForever.Game.Static.Combat;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogCCStateBreak : ICombatLog
    {
        public CombatLogType Type => CombatLogType.CCStateBreak;

        public uint CasterId { get; set; }
        public byte State { get; set; } // 5u

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CasterId);
            writer.Write(State, 5u);
        }
    }
}
