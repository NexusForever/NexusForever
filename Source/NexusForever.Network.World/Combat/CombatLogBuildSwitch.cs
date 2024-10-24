using NexusForever.Game.Static.Combat;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogBuildSwitch : ICombatLog
    {
        public CombatLogType Type => CombatLogType.BuildSwitch;

        public uint UnitId { get; set; }
        public byte NewSpecIndex { get; set; } // 3u

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(NewSpecIndex, 3u);
        }
    }
}
