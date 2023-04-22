using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.Server0811)]
    public class Server0811 : IWritable
    {
        public uint CastingId { get; set; }
        public List<uint> CasterId { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CastingId);
            writer.Write(CasterId.Count, 32u);
            CasterId.ForEach(c => writer.Write(c));
        }
    }
}
