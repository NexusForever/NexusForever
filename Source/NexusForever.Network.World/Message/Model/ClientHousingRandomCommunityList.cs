using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingRandomCommunityList)]
    public class ClientHousingRandomCommunityList : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}
