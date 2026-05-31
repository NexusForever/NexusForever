using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Chat
{
    [Message(GameMessageOpcode.ClientChatKick)]
    public class ClientChatKick : IReadable
    {
        public Channel Channel { get; private set; } = new();
        public string CharacterName { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Channel.Read(reader);
            CharacterName = reader.ReadWideString();
        }
    }
}
