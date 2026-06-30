using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ServerFriendshipAdd)]
    public class ServerFriendshipAdd : IWritable
    {
        public FriendData Friend { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            Friend.Write(writer);
        }
    }
}
