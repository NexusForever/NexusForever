using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientFriendshipIgnoreStrangersState)]
    public class ClientFriendshipIgnoreStrangersState : IReadable
    {
        public bool IgnoreStrangerInvites { get; private set; }

        public void Read(GamePacketReader reader)
        {
            IgnoreStrangerInvites = reader.ReadBit();
        }
    }
}
