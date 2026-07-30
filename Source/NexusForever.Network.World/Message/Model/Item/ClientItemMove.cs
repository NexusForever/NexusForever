using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
{
    [Message(GameMessageOpcode.ClientItemMove)]
    public class ClientItemMove : IReadable
    {
        public ItemLocation From { get; } = new();
        public ItemLocation To { get; } = new();

        public void Read(GamePacketReader reader)
        {
            From.Read(reader);
            To.Read(reader);
        }
    }
}
