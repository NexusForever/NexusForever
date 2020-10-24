using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientActivateUnitInteraction)]
    public class ClientActivateUnitInteraction : IReadable
    {
        public uint ClientUniqueId { get; private set; } // first value of 0x7FD response, probably global increment
        public uint ActivateUnitId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ClientUniqueId  = reader.ReadUInt();
            ActivateUnitId  = reader.ReadUInt();
        }
    }
}
