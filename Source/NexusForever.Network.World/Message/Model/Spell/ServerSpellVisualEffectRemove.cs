using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Spell
{
    // Removes a specific spell visual effect from the game.
    [Message(GameMessageOpcode.ServerSpellVisualEffectRemove)]
    public class ServerSpellVisualEffectRemove : IWritable
    {
        public uint SpellVisualEffectUniqueId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(SpellVisualEffectUniqueId, 18u);
        }
    }
}

