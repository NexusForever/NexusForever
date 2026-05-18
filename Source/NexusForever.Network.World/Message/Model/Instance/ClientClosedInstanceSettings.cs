using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Instance
{
    [Message(GameMessageOpcode.ClientClosedInstanceSettings)]
    public class ClientClosedInstanceSettings : IReadable
    {
        public uint InstancePortalUnitId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            InstancePortalUnitId = reader.ReadUInt();
        }
    }
}
