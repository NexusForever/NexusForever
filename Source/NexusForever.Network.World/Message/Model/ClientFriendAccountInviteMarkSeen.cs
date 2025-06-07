using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientFriendAccountInviteMarkSeen)]
    public class ClientFriendAccountInviteMarkSeen : IReadable
    {
        public ulong AccountFriendInviteId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            AccountFriendInviteId = reader.ReadULong();
        }
    }
}
