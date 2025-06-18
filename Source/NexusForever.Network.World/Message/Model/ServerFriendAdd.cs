using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
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
