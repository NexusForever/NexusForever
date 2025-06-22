using NexusForever.Game.Static.Friend;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerFriendTypeUpdate)]
    public class ServerFriendTypeUpdate : IWritable
    {
        public ulong FriendshipId { get; set; }
        public FriendshipType Type { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(FriendshipId);
            writer.Write(Type, 4u);
        }
    }
}
