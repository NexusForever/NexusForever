using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ClientFriendshipAccountInviteMarkSeen)]
    public class ClientFriendshipAccountInviteMarkSeen : IReadable
    {
        public ulong AccountFriendInviteId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            AccountFriendInviteId = reader.ReadULong();
        }
    }
}
