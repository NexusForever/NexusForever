using NexusForever.Game.Static.Friend;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientFriendRemoveByName)]
    public class ClientFriendRemoveByName : IReadable
    {
        public string Name { get; private set; }
        public string RealmName { get; private set; }
        public FriendshipType Type { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Name = reader.ReadWideString();
            RealmName = reader.ReadWideString();
            Type = reader.ReadEnum<FriendshipType>(4u);
        }
    }
}
