using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerSpellBuffsRemoved)]
    public class ServerSpellBuffsRemoved : IWritable
    {
        public uint CastingId { get; set; }
        public List<uint> SpellTargets { get; set; } = new List<uint>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CastingId);
            writer.Write(SpellTargets.Count, 32u);
            SpellTargets.ForEach(c => writer.Write(c));
        }
    }
}
