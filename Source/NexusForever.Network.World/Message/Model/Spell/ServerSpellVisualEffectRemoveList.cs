using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Spell
{
    [Message(GameMessageOpcode.ServerSpellVisualEffectRemoveList)]
    public class ServerSpellVisualEffectRemoveList : IWritable
    {
        List<uint> SpellVisualEffectUniqueIds { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(SpellVisualEffectUniqueIds.Count);
            SpellVisualEffectUniqueIds.ForEach(id => writer.Write(id));
        }
    }
}
