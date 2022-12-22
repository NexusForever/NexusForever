using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
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
