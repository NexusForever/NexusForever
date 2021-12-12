using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientActivateUnitInteraction)]
    public class ClientActivateUnitInteraction : IReadable
    {
        public uint ClientUniqueId { get; private set; } // first value of 0x7FD response, probably global increment
        public uint ActivateUnitId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ClientUniqueId = reader.ReadUInt();
            ActivateUnitId = reader.ReadUInt();
        }
    }
}