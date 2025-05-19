using NexusForever.Game.Static.Friend;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientFriendRemoveByIdentity)]
    public class ClientFriendRemoveByIdentity : IReadable
    {
        public TargetPlayerIdentity PlayerIdentity { get; private set; } = new();
        public FriendshipType Type { get; private set; }

        public void Read(GamePacketReader reader)
        {
            PlayerIdentity.Read(reader);
            Type = reader.ReadEnum<FriendshipType>(4u);
        }
    }
}
