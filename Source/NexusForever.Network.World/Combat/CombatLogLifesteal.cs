using NexusForever.Game.Static.Combat;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogLifesteal : ICombatLog
    {
        public CombatLogType Type => CombatLogType.Lifesteal;

        public uint Unknown0 { get; set; }
        public uint Unknown1 { get; set; }
        public uint Unknown2 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0);
            writer.Write(Unknown1);
            writer.Write(Unknown2);
        }
    }
}
