using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientChatList)]
    public class ClientChatList : IReadable
    {
        public Channel Channel { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Channel = new Channel();
            Channel.Read(reader);
        }
    }
}
