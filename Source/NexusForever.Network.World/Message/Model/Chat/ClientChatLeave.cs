using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Chat
{
    [Message(GameMessageOpcode.ClientChatLeave)]
    public class ClientChatLeave : IReadable
    {
        public Channel Channel { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Channel = new Channel();
            Channel.Read(reader);
        }
    }
}
