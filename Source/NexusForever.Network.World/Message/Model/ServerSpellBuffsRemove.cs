using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerSpellBuffRemove)] // removes same buff base on the spell CastingId from a list of targets
    public class ServerSpellBuffRemove : IWritable
    {
        public uint ServerUniqueId { get; set; }
        public List<uint> Targets { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ServerUniqueId);
            writer.Write(Targets.Count, 32u);
            Targets.ForEach(c => writer.Write(c));
        }
    }
}
