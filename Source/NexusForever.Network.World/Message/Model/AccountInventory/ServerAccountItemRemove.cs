using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.AccountInventory
{
    [Message(GameMessageOpcode.ServerAccountItemRemove)]
    public class ServerAccountItemRemove : IWritable
    {
        public ulong UserInventoryId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UserInventoryId);
        }
    }
}
