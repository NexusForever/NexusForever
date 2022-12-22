using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerSpellBuffRemove)]
    public class ServerSpellBuffRemove : IWritable
    {
        public uint CastingId { get; set; }
        public uint CasterId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CastingId);
            writer.Write(CasterId);
        }
    }
}
