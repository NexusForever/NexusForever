using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientFriendPersonalAwayMessage)]
    public class ClientFriendPersonalAwayMessage : IReadable
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
