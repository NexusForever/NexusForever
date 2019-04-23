using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientItemDelete)]
    public class ClientItemDelete : IReadable
    {
        public ItemLocation From { get; } = new ItemLocation();

        public void Read(GamePacketReader reader)
        {
            From.Read(reader);
        }
    }
}
