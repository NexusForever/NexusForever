using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ClientFriendshipAccountSetPublicDisplayName)]
    public class ClientFriendshipAccountSetPublicDisplayName : IReadable
    {
        public string PublicDisplayName { get; private set; }

        public void Read(GamePacketReader reader)
        {
            PublicDisplayName = reader.ReadWideString();
        }
    }
}
