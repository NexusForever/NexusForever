using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerFriendRemove)]
    public class ServerFriendRemove : IWritable
    {
        public ulong FriendshipId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(FriendshipId);
        }
    }
}
