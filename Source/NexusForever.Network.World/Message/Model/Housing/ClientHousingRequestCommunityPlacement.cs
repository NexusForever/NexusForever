using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ClientHousingRequestCommunityPlacement)]
    public class ClientHousingRequestCommunityPlacement : IReadable
    {
        public Identity ResidenceIdentity { get; } = new();
        public uint PropertyIndex { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ResidenceIdentity.Read(reader);
            PropertyIndex = reader.ReadUInt();
        }
    }
}
