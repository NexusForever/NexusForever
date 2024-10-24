using NexusForever.Game.Static.Combat;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogDispel : ICombatLog
    {
        public CombatLogType Type => CombatLogType.Dispel;

        public bool BRemovesSingleInstance { get; set; }
        public uint InstancesRemoved { get; set; }
        public uint SpellRemovedId { get; set; } // 18u

        public void Write(GamePacketWriter writer)
        {
            writer.Write(BRemovesSingleInstance);
            writer.Write(InstancesRemoved);
            writer.Write(SpellRemovedId, 18u);
        }
    }
}
