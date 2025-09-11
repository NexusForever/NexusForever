using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientResetSingleInstance)]
    public class ClientResetSingleInstance : IReadable
    {
        public uint InstancePortalUnitId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            InstancePortalUnitId = reader.ReadUInt();
        }
    }
}
