using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientChatKick)]
    public class ClientChatKick : IReadable
    {
        public Channel Channel { get; private set; }
        public string CharacterName { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Channel = new Channel();
            Channel.Read(reader);
            CharacterName = reader.ReadWideString();
        }
    }
}
