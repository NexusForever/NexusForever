using NexusForever.Game.Static.Combat;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogModifying : ICombatLog
    {
        public CombatLogType Type => CombatLogType.Modifying;

        public uint CasterId { get; set; }
        public uint HostItemId { get; set; } // 18u
        public uint SocketedItemId { get; set; }
        public uint UnsocketedItemId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CasterId);
            writer.Write(HostItemId, 18u);
            writer.Write(SocketedItemId);
            writer.Write(UnsocketedItemId);
        }
    }
}
