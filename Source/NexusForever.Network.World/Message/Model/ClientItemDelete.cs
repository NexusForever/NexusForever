using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientItemDelete)]
    public class ClientItemDelete : IReadable
    {
        public ItemLocation From { get; } = new();

        public void Read(GamePacketReader reader)
        {
            From.Read(reader);
        }
    }
}
