using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ClientHousingVendorList)]
    public class ClientHousingVendorList : IReadable
    {
        public Identity ResidenceIdentity { get; private set; } = new();
        public byte ResidenceIsBroker { get; private set; } // 0 = normal residences, 1 is when is ResidenceType.Broker

        public void Read(GamePacketReader reader)
        {
            ResidenceIdentity.Read(reader);
            ResidenceIsBroker = reader.ReadByte(2u);
        }
    }
}
