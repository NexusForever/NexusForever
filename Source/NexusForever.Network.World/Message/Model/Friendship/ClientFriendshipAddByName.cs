using NexusForever.Game.Static.Friendship;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ClientFriendshipAddByName)]
    public class ClientFriendshipAddByName : IReadable
    {
        public string Name { get; private set; }
        public string RealmName { get; private set; }
        public FriendshipType Type { get; private set; }
        public string Note { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Name      = reader.ReadWideString();
            RealmName = reader.ReadWideString();
            Type      = reader.ReadEnum<FriendshipType>(4);
            Note      = reader.ReadWideString();
        }
    }
}
