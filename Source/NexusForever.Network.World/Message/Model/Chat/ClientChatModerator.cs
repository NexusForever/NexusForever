using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Chat
{
    [Message(GameMessageOpcode.ClientChatModerator)]
    public class ClientChatModerator : IReadable
    {
        public Channel Channel { get; private set; }
        public string CharacterName { get; private set; }
        public bool MakeModerator { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Channel       = new Channel();
            Channel.Read(reader);
            CharacterName = reader.ReadWideString();
            MakeModerator = reader.ReadBit();
        }
    }
}
