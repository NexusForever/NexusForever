using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientSpellStopCast, MessageDirection.Client)]
    public class ClientSpellStopCast : IReadable
    {
        public uint CastingId { get; private set; } // first value of 0x7FD response, probably global increment
        public ushort Unknown1 { get; private set; }
        public bool Unknown2 { get; private set; }

        public void Read(GamePacketReader reader)
        {
            CastingId = reader.ReadUInt();
            Unknown1 = reader.ReadUShort(9);
            Unknown2 = reader.ReadBit();
        }
    }
}
