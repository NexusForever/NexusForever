using NexusForever.Game.Static.Friend;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
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
