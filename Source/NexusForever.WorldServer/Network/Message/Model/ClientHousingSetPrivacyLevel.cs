using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Housing.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingResidencePrivacyLevel)]
    public class ClientHousingSetPrivacyLevel : IReadable
    {
        public ResidencePrivacyLevel PrivacyLevel { get; private set; }

        public void Read(GamePacketReader reader)
        {
            PrivacyLevel = reader.ReadEnum<ResidencePrivacyLevel>(32u);
        }
    }
}
