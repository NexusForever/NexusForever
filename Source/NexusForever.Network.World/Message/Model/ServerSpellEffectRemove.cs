using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerSpellEffectRemove)]
    public class ServerSpellEffectRemove : IWritable
    {
        public uint Spell4EffectId { get; set; }
        public uint CastingId { get; set; }
        public uint SpellEffectUniqueId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Spell4EffectId);
            writer.Write(CastingId);
            writer.Write(SpellEffectUniqueId);
        }
    }
}
