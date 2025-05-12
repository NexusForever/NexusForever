using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientCastObjectiveAbility)]
    public class ClientCastObjectiveAbility : IReadable
    {
        public uint ClientSpellCastUniqueId { get; set; }
        public uint Spell4Id { get; set; }
        public uint TargetUnitId { get; set; }
        public Position Position { get; set; } = new Position();

        public void Read(GamePacketReader reader)
        {
            ClientSpellCastUniqueId = reader.ReadUInt();
            Spell4Id = reader.ReadUInt(18);
            TargetUnitId = reader.ReadUInt();
            Position.Read(reader);
        }
    }
}