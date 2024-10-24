using NexusForever.Game.Static.Combat;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogAbsorption : ICombatLog
    {
        public CombatLogType Type => CombatLogType.Absorption;

        public uint AbsorptionAmount { get; set; }
        public CombatLogCastData CastData { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AbsorptionAmount);
            CastData.Write(writer);
        }
    }
}
