using NexusForever.Game.Static.Combat;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogPet : ICombatLog
    {
        public CombatLogType Type => CombatLogType.Pet;

        public bool BDismissed { get; set; }
        public bool BKilled { get; set; }
        public CombatLogCastData CastData { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(BDismissed);
            writer.Write(BKilled);
            CastData.Write(writer);
        }
    }
}
