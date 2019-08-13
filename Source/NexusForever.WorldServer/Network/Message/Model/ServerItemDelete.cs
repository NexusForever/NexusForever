using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerItemDelete)]
    public class ServerItemDelete : IWritable
    {
        public ulong Guid { get; set; }
        public ItemUpdateReason Reason { get; set; } = ItemUpdateReason.Loot;

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guid);
            writer.Write(Reason, 6u);
        }
    }
}
