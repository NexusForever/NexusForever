using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Spell
{
    [Message(GameMessageOpcode.ClientSpellClientSideInteraction)]
    public class ClientSpellClientSideInteraction : IReadable
    {
        public uint ServerUniqueId { get; private set; }
        public byte CSIResponse { get; private set; } // value of 2 or 0
        public uint Spell4IdBaseIdBaseSpell { get; private set; } // Is actually spell4IdBaseIdBaseSpell from spell plus 1, see spell4 tbl

        public void Read(GamePacketReader reader)
        {
            ServerUniqueId = reader.ReadUInt();
            CSIResponse = reader.ReadByte(3u);
            Spell4IdBaseIdBaseSpell = reader.ReadUInt();
        }
    }
}
