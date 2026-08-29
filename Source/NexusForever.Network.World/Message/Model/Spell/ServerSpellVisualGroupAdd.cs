using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Spell
{
    [Message(GameMessageOpcode.ServerSpellVisualGroupAdd)]
    public class ServerSpellVisualGroupAdd : IWritable
    {
        public uint UnitId { get; set; }
        public ushort Spell4VisualGroupId { get; set; }
        public uint Unknown { get; set; } // mostly 0, sometimes 1 TODO: research more
        public List<uint> SpellVisualEffectUniqueIds { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Spell4VisualGroupId);
            writer.Write(Unknown);
            writer.Write(SpellVisualEffectUniqueIds.Count);
            SpellVisualEffectUniqueIds.ForEach(id => writer.Write(id));
        }
    }
}
