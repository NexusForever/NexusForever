using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ServerFriendAdd)]
    public class ServerFriendAdd : IWritable
    {
        public FriendData Friend { get; set; } = new FriendData();

        public void Write(GamePacketWriter writer)
        {
            Friend.Write(writer);
        }
    }
}
