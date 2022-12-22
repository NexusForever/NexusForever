using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientChatPassword)]
    public class ClientChatPassword : IReadable
    {
        public Channel Channel { get; private set; }
        public string Password { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Channel  = new Channel();
            Channel.Read(reader);
            Password = reader.ReadWideString();
        }
    }
}
