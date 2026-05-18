using NexusForever.Game.Static.Combat;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogExperience : ICombatLog
    {
        public CombatLogType Type => CombatLogType.Experience;

        public uint UnitId { get; set; }
        public uint Xp { get; set; }
        public uint RestXp { get; set; }
        public uint ElderPoints { get; set; }
        public uint RestElderPoints { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Xp);
            writer.Write(RestXp);
            writer.Write(ElderPoints);
            writer.Write(RestElderPoints);
        }
    }
}
