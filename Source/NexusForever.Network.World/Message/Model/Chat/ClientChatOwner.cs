using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Chat
{
    [Message(GameMessageOpcode.ClientChatOwner)]
    public class ClientChatOwner : IReadable
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
