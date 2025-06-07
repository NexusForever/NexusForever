using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientFriendAccountSetPublicNote)]
    public class ClientFriendAccountSetPublicNote : IReadable
    {
        public string PublicNote { get; private set; }

        public void Read(GamePacketReader reader)
        {
            PublicNote = reader.ReadWideString();
        }
    }
}
