using NexusForever.Game.Static.Friendship;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    public class FriendData : IWritable
    {
        public ulong FriendshipId { get; set; }
        public Identity PlayerIdentity { get; set; } = new();
        public string Note { get; set; } = "";
        public FriendshipType Type { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(FriendshipId);
            PlayerIdentity.Write(writer);
            writer.WriteStringWide(Note);
            writer.Write(Type, 4u);
        }
    }
}
