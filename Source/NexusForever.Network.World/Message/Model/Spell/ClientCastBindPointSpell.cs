using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Spell
{
    [Message(GameMessageOpcode.ClientCastBindPointSpell)]
    public class ClientCastBindPointSpell : IReadable
    {
        public uint ClientSpellCastUniqueId { get; private set; }
        public uint UnitId { get; private set; } // Bind point units

        public void Read(GamePacketReader reader)
        {
            ClientSpellCastUniqueId = reader.ReadUInt();
            UnitId = reader.ReadUInt();
        }
    }
}
