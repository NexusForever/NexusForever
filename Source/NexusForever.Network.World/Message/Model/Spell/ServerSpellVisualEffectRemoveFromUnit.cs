using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Spell
{
    // Removes all spell visual effects from a unit
    [Message(GameMessageOpcode.ServerSpellVisualEffectRemoveFromUnit)]
    public class ServerSpellVisualEffectRemoveFromUnit : IWritable
    {
        public uint TargetUnitId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TargetUnitId, 18u);
        }
    }
}

