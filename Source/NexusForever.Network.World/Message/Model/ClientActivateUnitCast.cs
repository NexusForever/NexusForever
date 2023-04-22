using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientActivateUnitCast)]
    public class ClientActivateUnitCast : IReadable
    {
        public uint ClientUniqueId { get; private set; }
        public uint ActivateUnitId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ClientUniqueId  = reader.ReadUInt();
            ActivateUnitId  = reader.ReadUInt();
        }
    }
}
