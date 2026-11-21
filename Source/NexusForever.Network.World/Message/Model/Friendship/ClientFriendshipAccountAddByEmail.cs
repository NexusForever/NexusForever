using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ClientFriendshipAccountAddByEmail)]
    public class ClientFriendshipAccountAddByEmail : IReadable
    {
        public string Email { get; private set; }
        public string Note { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Email = reader.ReadWideString();
            Note  = reader.ReadWideString();
        }
    }
}
