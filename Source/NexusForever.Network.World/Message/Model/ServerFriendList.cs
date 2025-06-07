using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
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
