using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerFriendAccountRemoved)]
    public class ServerFriendAccountRemoved : IWritable
    {
        public ulong AccountFriendId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AccountFriendId);
        }
    }
}
