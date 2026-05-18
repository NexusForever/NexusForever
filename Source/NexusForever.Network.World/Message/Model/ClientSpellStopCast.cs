using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientSpellStopCast)]
    public class ClientSpellStopCast : IReadable
    {
        public uint CastingId { get; private set; } // first value of 0x7FD response, probably global increment
        public CastResult CastResult { get; set; }
        public bool Unknown2 { get; private set; }

        public void Read(GamePacketReader reader)
        {
            CastingId  = reader.ReadUInt();
            CastResult = reader.ReadEnum<CastResult>(9u);
            Unknown2   = reader.ReadBit();
        }
    }
}
