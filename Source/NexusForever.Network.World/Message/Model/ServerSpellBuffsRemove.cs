using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerSpellBuffRemoveFromTargetList)] // removes same buff base on the spell CastingId from a list of targets
    public class ServerSpellBuffRemoveFromTargetList : IWritable
    {
        public uint CastingId { get; set; }
        public List<uint> Targets { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CastingId);
            writer.Write(Targets.Count, 32u);
            Targets.ForEach(c => writer.Write(c));
        }
    }
}
