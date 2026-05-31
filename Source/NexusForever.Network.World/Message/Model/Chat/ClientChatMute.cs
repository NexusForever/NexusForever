using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Chat
{
    [Message(GameMessageOpcode.ClientChatMute)]
    public class ClientChatMute : IReadable
    {
        public Channel Channel { get; private set; }
        public string CharacterName { get; private set; } // if left empty, mutes the entire channel
        public bool Muted { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Channel       = new Channel();
            Channel.Read(reader);
            CharacterName = reader.ReadWideString();
            Muted         = reader.ReadBit();
        }
    }
}
