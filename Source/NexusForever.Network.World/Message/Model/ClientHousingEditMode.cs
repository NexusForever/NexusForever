using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingEditMode)]
    public class ClientHousingEditMode : IReadable
    {
        public TargetPlayerIdentity TargetPlayerIdentity { get; } = new();
        public bool Enabled { get; private set; }

        public void Read(GamePacketReader reader)
        {
            TargetPlayerIdentity.Read(reader);
            Enabled = reader.ReadBit();
        }
    }
}
