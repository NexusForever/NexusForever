using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ServerFriendList)]
    public class ServerFriendList : IWritable
    {
        public List<FriendData> Friends { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Friends.Count, 16u);
            Friends.ForEach(f => f.Write(writer));
        }
    }
}
