using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerSpellUpdateEffectDuration)]
    public class ServerSpellUpdateEffectDuration : IWritable
    {
        public uint ServerUniqueId { get; set; }
        public uint SpellEffectUniqueId { get; set; }
        public uint TimeRemaining { get; set; }
        public uint TargetUnitId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ServerUniqueId);
            writer.Write(SpellEffectUniqueId);
            writer.Write(TimeRemaining);
            writer.Write(TargetUnitId);
        }
    }
}
