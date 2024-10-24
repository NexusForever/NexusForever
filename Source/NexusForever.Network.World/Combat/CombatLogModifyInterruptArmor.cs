using NexusForever.Game.Static.Combat;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogModifyInterruptArmor : ICombatLog
    {
        public CombatLogType Type => CombatLogType.ModifyInterruptArmor;

        public uint Amount { get; set; }
        public CombatLogCastData CastData { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Amount);
            CastData.Write(writer);
        }
    }
}
