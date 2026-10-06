using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Spell
{
    [Message(GameMessageOpcode.ClientCastServiceToken)]
    public class ClientCastServiceToken : IReadable
    {
        public uint Spell4Id { get; private set; }
        public uint ClientSpellCastUniqueId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Spell4Id = reader.ReadUInt();
            ClientSpellCastUniqueId = reader.ReadUInt();
        }
    }
}
