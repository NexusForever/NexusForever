using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.Server07FA)]
    public class Server07FA : IWritable
    {
        public uint CastingId { get; set; }
        public ushort Unknown5 { get; set; }
        public uint CasterId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CastingId);
            writer.Write(Unknown5, 9u);
            writer.Write(CasterId);
        }
    }
}

