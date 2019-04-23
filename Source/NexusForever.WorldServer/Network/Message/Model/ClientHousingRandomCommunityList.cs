using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingRandomCommunityList)]
    public class ClientHousingRandomCommunityList : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}
