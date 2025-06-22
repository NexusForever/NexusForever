using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientFriendshipSetPublicDisplayName)]
    public class ClientFriendshipSetPublicDisplayName : IReadable
    {
        public string PublicDisplayName { get; private set; }

        public void Read(GamePacketReader reader)
        {
            PublicDisplayName = reader.ReadWideString();
        }
    }
}
