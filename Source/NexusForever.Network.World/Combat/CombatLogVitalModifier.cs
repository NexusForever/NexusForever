using NexusForever.Game.Static.Combat;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogVitalModifier : ICombatLog
    {
        public CombatLogType Type => CombatLogType.VitalModifier;

        public float Amount { get; set; }
        public uint VitalModified { get; set; } // 5u - TODO: Replace with Vital enum
        public bool BShowCombatLog { get; set; }
        public CombatLogCastData CastData { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Amount);
            writer.Write(VitalModified, 5u);
            writer.Write(BShowCombatLog);
            CastData.Write(writer);
        }
    }
}
