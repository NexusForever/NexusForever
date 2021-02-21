using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
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
