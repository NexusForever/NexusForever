using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ClientFriendshipAccountSetPublicNote)]
    public class ClientFriendshipAccountSetPublicNote : IReadable
    {
        public string PublicNote { get; private set; }

        public void Read(GamePacketReader reader)
        {
            PublicNote = reader.ReadWideString();
        }
    }
}
