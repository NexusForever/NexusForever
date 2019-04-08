using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientItemMove)]
    public class ClientItemMove : IReadable
    {
        public ItemLocation From { get; } = new ItemLocation();
        public ItemLocation To { get; } = new ItemLocation();

        public void Read(GamePacketReader reader)
        {
            From.Read(reader);
            To.Read(reader);
        }
    }
}
