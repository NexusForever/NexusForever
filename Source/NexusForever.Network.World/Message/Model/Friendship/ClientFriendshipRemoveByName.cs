using NexusForever.Game.Static.Friendship;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ClientFriendshipRemoveByName)]
    public class ClientFriendshipRemoveByName : IReadable
    {
        public string Name { get; private set; }
        public string RealmName { get; private set; }
        public FriendshipType Type { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Name      = reader.ReadWideString();
            RealmName = reader.ReadWideString();
            Type      = reader.ReadEnum<FriendshipType>(4u);
        }
    }
}
