using NexusForever.Game.Static.Housing;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
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
