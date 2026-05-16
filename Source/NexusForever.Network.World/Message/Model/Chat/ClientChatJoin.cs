using NexusForever.Game.Static.Chat;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Chat
{
    [Message(GameMessageOpcode.ClientChatJoin)]
    public class ClientChatJoin : IReadable
    {
        public ChatChannelType Type { get; private set; }
        public string Name { get; private set; }
        public string Password { get; private set; }
        public uint Order { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Type     = reader.ReadEnum<ChatChannelType>(14u);
            Name     = reader.ReadWideString();
            Password = reader.ReadWideString();
            Order    = reader.ReadUInt();
        }
    }
}
