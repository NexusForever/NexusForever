using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ClientFriendAccountRemove)]
    public class ClientFriendAccountRemove : IReadable
    {
        public ulong AccountFriendId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            AccountFriendId = reader.ReadULong();
        }
    }
}
