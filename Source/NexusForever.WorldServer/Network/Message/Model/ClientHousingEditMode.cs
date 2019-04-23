using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingEditMode)]
    public class ClientHousingEditMode : IReadable
    {
        public TargetPlayerIdentity TargetPlayerIdentity { get; } = new TargetPlayerIdentity();
        public bool Enabled { get; private set; }

        public void Read(GamePacketReader reader)
        {
            TargetPlayerIdentity.Read(reader);
            Enabled = reader.ReadBit();
        }
    }
}
