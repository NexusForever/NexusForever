using NexusForever.Game.Static.Combat;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogDeflect : ICombatLog
    {
        public CombatLogType Type => CombatLogType.Deflect;

        public bool BMultiHit { get; set; }
        public CombatLogCastData CastData { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(BMultiHit);
            CastData.Write(writer);
        }
    }
}
