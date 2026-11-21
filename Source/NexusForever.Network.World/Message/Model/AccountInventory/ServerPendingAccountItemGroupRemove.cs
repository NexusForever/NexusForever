using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.AccountInventory
{
    [Message(GameMessageOpcode.ServerPendingAccountItemGroupRemove)]
    public class ServerPendingAccountItemGroupRemove : IWritable
    {
        public ulong PendingItemGroupId { get; set; } // a bit of a guess
        public bool Unused { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PendingItemGroupId);
            writer.Write(Unused);
        }
    }
}
