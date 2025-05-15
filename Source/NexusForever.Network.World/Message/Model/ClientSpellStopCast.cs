using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientSpellStopCast)]
    public class ClientSpellStopCast : IReadable
    {
        public uint ServerUniqueId { get; private set; } // first value of 0x7FD response, probably global increment
        public CastResult CastResult { get; set; }
        public bool Cancelled { get; private set; } // true = cancelled, false = released

        public void Read(GamePacketReader reader)
        {
            ServerUniqueId = reader.ReadUInt();
            CastResult = reader.ReadEnum<CastResult>(9u);
            Cancelled   = reader.ReadBit();
        }
    }
}
