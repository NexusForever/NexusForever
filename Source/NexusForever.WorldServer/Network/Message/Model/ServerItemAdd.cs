using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
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
