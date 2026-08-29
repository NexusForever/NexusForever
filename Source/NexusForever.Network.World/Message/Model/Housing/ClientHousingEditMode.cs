using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ClientHousingEditMode)]
    public class ClientHousingEditMode : IReadable
    {
        public Identity ResidenceIdentity { get; } = new(); // Can be a residence the player has rights to edit, a community residence, or war party plot
        public bool Enabled { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ResidenceIdentity.Read(reader);
            Enabled = reader.ReadBit();
        }
    }
}
