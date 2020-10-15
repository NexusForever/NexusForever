using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientAccountItemBind)]
    public class ClientAccountItemBind : IReadable
    {
        public ulong UserInventoryId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            UserInventoryId  = reader.ReadULong();
        }
    }
}
