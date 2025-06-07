using NexusForever.Game.Static.Friend;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
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
