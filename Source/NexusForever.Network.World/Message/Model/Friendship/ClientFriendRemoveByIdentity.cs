using NexusForever.Game.Static.Friendship;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ClientFriendRemoveByIdentity)]
    public class ClientFriendRemoveByIdentity : IReadable
    {
        public Identity PlayerIdentity { get; private set; } = new();
        public FriendshipType Type { get; private set; }

        public void Read(GamePacketReader reader)
        {
            PlayerIdentity.Read(reader);
            Type = reader.ReadEnum<FriendshipType>(4u);
        }
    }
}
