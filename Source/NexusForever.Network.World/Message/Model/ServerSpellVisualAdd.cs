using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerSpellVisualAdd)]
    public class ServerSpellVisualAdd : IWritable
    {
        public uint SpellVisualEffectClientId { get; set; }
        public uint UnitId { get; set; }
        public uint VisualEffectId { get; set; }
        public uint VisualEffectIdSound { get; set; }
        public uint Spell4VisualId { get; set; }
        public uint Unk14 { get; set; }
        public Position position { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(SpellVisualEffectClientId);
            writer.Write(UnitId);
            writer.Write(VisualEffectId, 11);
            writer.Write(VisualEffectIdSound, 11);
            writer.Write(Spell4VisualId, 11);
            writer.Write(Unk14);
            position.Write(writer);
        }
    }
}
