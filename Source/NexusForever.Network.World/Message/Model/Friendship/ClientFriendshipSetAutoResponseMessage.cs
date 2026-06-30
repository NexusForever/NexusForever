using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ClientFriendshipSetAutoResponseMessage)]
    public class ClientFriendshipSetAutoResponseMessage : IReadable
    {
        public string AwayMessage { get; private set; }
        public string BusyMessage { get; private set; }
        
        public void Read(GamePacketReader reader)
        {
            AwayMessage = reader.ReadWideString();
            BusyMessage = reader.ReadWideString();
        }
    }
}
