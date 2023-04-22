using NexusForever.Game.Static.Housing;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
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
