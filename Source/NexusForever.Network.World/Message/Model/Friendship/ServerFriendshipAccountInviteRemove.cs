using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ServerFriendshipAccountInviteRemove)]
    public class ServerFriendshipAccountInviteRemove : IWritable
    {
        public uint AccountId { get; set; }
        public ulong AccountFriendInviteId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AccountId);
            writer.Write(AccountFriendInviteId);
        }
    }
}
