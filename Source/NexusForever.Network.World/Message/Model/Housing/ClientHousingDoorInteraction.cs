using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ClientHousingDoorInteraction)]
    public class ClientHousingDoorInteraction : IReadable
    {
        public Identity ResidenceIdentity { get; } = new();

        public void Read(GamePacketReader reader)
        {
            ResidenceIdentity.Read(reader);
        }
    }
}
