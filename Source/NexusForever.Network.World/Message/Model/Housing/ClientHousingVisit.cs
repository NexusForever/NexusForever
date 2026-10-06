using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ClientHousingVisit)]
    public class ClientHousingVisit : IReadable
    {
        public ulong Unused { get; private set; } // always 0
        public Identity IdentityToVisit { get; } = new(); // can be a player Identity or a residence Identity
        public string PlayerToVisitName { get; private set; }
        public Identity CommunityToVisitIdentity { get; } = new();
        public string CommunityToVisitName { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Unused = reader.ReadULong();
            IdentityToVisit.Read(reader);
            PlayerToVisitName = reader.ReadWideString();
            CommunityToVisitIdentity.Read(reader);
            CommunityToVisitName = reader.ReadWideString();
        }
    }
}
