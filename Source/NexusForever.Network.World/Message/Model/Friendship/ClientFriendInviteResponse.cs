using NexusForever.Game.Static.Friendship;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ClientFriendInviteResponse)]
    public class ClientFriendInviteResponse : IReadable
    {
        public ulong InviteId { get; private set; }
        public FriendshipResponse Response { get; private set; }

        public void Read(GamePacketReader reader)
        {
            InviteId = reader.ReadULong();
            Response  = reader.ReadEnum<FriendshipResponse>(3u);
        }
    }
}
