using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ClientFriendshipAccountInviteResponse)]
    public class ClientFriendshipAccountInviteResponse : IReadable
    {
        public ulong AccountFriendInviteId { get; private set; }
        public bool Response { get; private set; }

        public void Read(GamePacketReader reader)
        {
            AccountFriendInviteId = reader.ReadULong();
            Response              = reader.ReadBit();
        }
    }
}
