using NexusForever.Game.Static.Friendship;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ClientFriendAccountAddByName)]
    public class ServerFriendAccountAddByName : IWritable
    {
        public string Name { get; private set; }
        public string RealmName { get; set; }
        public FriendshipType Type { get; set; }
        public string Note { get; set; } // Optional note sent with invite

        public void Write(GamePacketWriter writer)
        {
            writer.WriteStringWide(Name);
            writer.WriteStringWide(RealmName);
            writer.Write(Type, 4u);
            writer.WriteStringWide(Note);
        }
    }
}
