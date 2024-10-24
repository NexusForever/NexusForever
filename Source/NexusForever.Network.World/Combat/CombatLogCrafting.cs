using NexusForever.Game.Static.Combat;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogCrafting : ICombatLog
    {
        public CombatLogType Type => CombatLogType.Crafting;

        public uint Unknown0 { get; set; }
        public uint Unknown1 { get; set; } // 18u

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0);
            writer.Write(Unknown1, 18u);
        }
    }
}
