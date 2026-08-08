using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ServerFriendshipRemove)]
    public class ServerFriendshipRemove : IWritable
    {
        public ulong FriendshipId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(FriendshipId);
        }
    }
}
