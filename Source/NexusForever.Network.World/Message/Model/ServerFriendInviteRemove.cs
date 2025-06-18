using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerFriendInviteRemove)]
    public class ServerFriendInviteRemove : IWritable
    {
        public ulong InviteId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(InviteId);
        }
    }
}
