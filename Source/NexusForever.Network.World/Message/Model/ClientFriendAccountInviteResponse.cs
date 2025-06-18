using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientFriendAccountInviteResponse)]
    public class ClientFriendAccountInviteResponse : IReadable
    {
        public ulong AccountFriendInviteId { get; private set; }
        public bool Accepted { get; private set; }

        public void Read(GamePacketReader reader)
        {
            AccountFriendInviteId = reader.ReadULong();
            Accepted = reader.ReadBit();
        }
    }
}
