using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerFriendAccountInviteRemove)]
    public class ServerFriendAccountInviteRemove : IWritable
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
