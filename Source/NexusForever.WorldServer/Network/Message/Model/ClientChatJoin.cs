using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Social.Static;

namespace NexusForever.WorldServer.Network.Message.Model
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
