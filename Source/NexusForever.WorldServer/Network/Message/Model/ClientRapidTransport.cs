using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientRapidTransport)]
    public class ClientRapidTransport : IReadable
    {
        public ushort TaxiNode { get; private set; }
        // not sure about the name - some increment
        public uint Time { get; private set; }

        public void Read(GamePacketReader reader)
        {
            TaxiNode  = reader.ReadUShort(14u);
            Time      = reader.ReadUInt();
        }
    }
}
