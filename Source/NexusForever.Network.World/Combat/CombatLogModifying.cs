using NexusForever.Game.Static.Combat;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogModifying : ICombatLog
    {
        public CombatLogType Type => CombatLogType.Modifying;

        public uint CasterId { get; set; }
        public uint HostItem2Id { get; set; } // 18u
        public uint ItemRemovedItem2Id { get; set; }
        public uint ItemAddedItem2Id { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CasterId);
            writer.Write(HostItem2Id, 18u);
            writer.Write(ItemRemovedItem2Id);
            writer.Write(ItemAddedItem2Id);
        }
    }
}
