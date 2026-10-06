using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.AccountInventory
{
    [Message(GameMessageOpcode.ServerPendingAccountItemAdd)]
    public class ServerPendingAccountItemAdd : IWritable
    {
        public PendingAccountItem PendingItem { get; set; }

        public void Write(GamePacketWriter writer)
        {
            PendingItem.Write(writer);
        }
    }
}
