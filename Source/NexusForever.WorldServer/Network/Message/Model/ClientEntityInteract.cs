using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientEntityInteract)]
    public class ClientEntityInteract : IReadable
    {
        public uint Guid { get; private set; }
        public byte Event { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Guid     = reader.ReadUInt();
            Event = reader.ReadByte(7);
        }
    }
}
