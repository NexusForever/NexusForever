using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
{
    [Message(GameMessageOpcode.ServerItemAdd)]
    public class ServerItemAdd : IWritable
    {
        public InventoryItem InventoryItem { get; set; }

        public void Write(GamePacketWriter writer)
        {
            InventoryItem.Write(writer);
        }
    }
}
