using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.AccountInventory
{
    [Message(GameMessageOpcode.ServerAccountItemInProgressRemove)]
    public class ServerAccountItemInProgressRemove : IWritable
    {
        public uint AccountId { get; set; } // is the AccountId sent ServerRealmInfo (0x3DB), filled by server but not used by client
        public ulong UserInventoryId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AccountId);
            writer.Write(UserInventoryId);
        }
    }
}
