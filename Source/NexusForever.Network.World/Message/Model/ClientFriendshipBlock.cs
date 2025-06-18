using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientFriendshipBlock)]
    public class ClientFriendshipBlock : IReadable
    {
        public bool BlockFriendRequests { get; private set; } 

        public void Read(GamePacketReader reader)
        {
            BlockFriendRequests = reader.ReadBit();
        }
    }
}
