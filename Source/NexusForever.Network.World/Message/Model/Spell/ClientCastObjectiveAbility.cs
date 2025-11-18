using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;

namespace NexusForever.Network.World.Message.Model.Spell
{
    [Message(GameMessageOpcode.ClientCastObjectiveAbility)]
    public class ClientCastObjectiveAbility : IReadable
    {
        public uint ClientSpellCastUniqueId { get; private set; }
        public uint Spell4Id { get; private set; }
        public uint TargetUnitId { get; private set; }
        public Position Position { get; private set; } = new Position();

        public void Read(GamePacketReader reader)
        {
            ClientSpellCastUniqueId = reader.ReadUInt();
            Spell4Id = reader.ReadUInt(18);
            TargetUnitId = reader.ReadUInt();
            Position.Read(reader);
        }
    }
}