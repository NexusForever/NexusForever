using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Housing.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingCommunityPrivacyLevel)]
    public class ClientHousingCommunityPrivacyLevel : IReadable
    {
        public TargetResidence TargetResidence { get; private set; } = new();
        public CommunityPrivacyLevel PrivacyLevel { get; private set; }

        public void Read(GamePacketReader reader)
        {
            TargetResidence.Read(reader);
            PrivacyLevel = reader.ReadEnum<CommunityPrivacyLevel>(1);
        }
    }
}
