using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ClientFriendshipAccountRemove)]
    public class ClientFriendshipAccountRemove : IReadable
    {
        public ulong AccountFriendId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            AccountFriendId = reader.ReadULong();
        }
    }
}
