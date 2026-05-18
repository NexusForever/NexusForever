using NexusForever.Game.Static.Combat;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogMount : ICombatLog
    {
        public CombatLogType Type => CombatLogType.Mount;

        public bool BDismounted { get; set; }
        public CombatLogCastData CastData { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(BDismounted);
            CastData.Write(writer);
        }
    }
}
