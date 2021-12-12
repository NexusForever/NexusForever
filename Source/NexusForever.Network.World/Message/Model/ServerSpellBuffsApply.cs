using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerSpellBuffsApply)]
    public class ServerSpellBuffsApply : IWritable
    {
        public class SpellTarget : IWritable
        {
            public uint ServerUniqueId { get; set; }
            public uint TargetId { get; set; }
            public uint InstanceCount { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(ServerUniqueId);
                writer.Write(TargetId);
                writer.Write(InstanceCount);
            }
        }

        public List<SpellTarget> spellTargets { get; set; } = new List<SpellTarget>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(spellTargets.Count, 32u);
            spellTargets.ForEach(i => i.Write(writer));
        }
    }
}
