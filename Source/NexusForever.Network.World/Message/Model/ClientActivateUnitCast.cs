using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientActivateUnitCast)]
    public class ClientActivateUnitCast : IReadable
    {
        public uint ClientSpellCastUniqueId { get; private set; }
        public uint UnitId { get; private set; } // Unit to activate

        public void Read(GamePacketReader reader)
        {
            ClientSpellCastUniqueId  = reader.ReadUInt();
            UnitId  = reader.ReadUInt();
        }
    }
}
