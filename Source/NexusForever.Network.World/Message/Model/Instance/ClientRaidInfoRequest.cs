using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Instance
{
    [Message(GameMessageOpcode.ClientRaidInfoRequest)]
    public class ClientRaidInfoRequest : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // Zero byte message
        }
    }
}
