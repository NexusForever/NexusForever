using NexusForever.Game.Static.Friendship;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ServerFriendshipTypeUpdate)]
    public class ServerFriendshipTypeUpdate : IWritable
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
