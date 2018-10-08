using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Social;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientChat, MessageDirection.Client)]
    public class ClientChat : IReadable
    {
        public ChatChannel Channel { get; private set; }
        public ulong Unknown0 { get; set; }
        public string Message { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Channel  = reader.ReadEnum<ChatChannel>(14);
            Unknown0 = reader.ReadULong();
            Message  = reader.ReadWideString();

            byte b = reader.ReadByte(5);
        }
    }
}
