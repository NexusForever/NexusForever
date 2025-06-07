using NexusForever.Game.Static.Friend;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class FriendData : IWritable
    {
        public ulong FriendshipId { get; set; }
        public TargetPlayerIdentity PlayerIdentity { get; set; } = new TargetPlayerIdentity();
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
