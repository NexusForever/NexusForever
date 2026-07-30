using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
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
