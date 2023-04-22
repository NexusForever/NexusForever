using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.Server07FB)]
    public class Server07FB : IWritable
    {
        public class UnknownStructure0 : IWritable
        {   
            public uint CasterId { get; set; }
            public byte Unknown4 { get; set; } = 0;
            public uint Unknown5 { get; set; } = 0;

            public void Write(GamePacketWriter writer)
            {
                writer.Write(CasterId);
                writer.Write(Unknown4, 4u);
                writer.Write(Unknown4);
            }
        }
        public uint CastingId { get; set; }

        public List<UnknownStructure0> unknownStructure0 { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CastingId);
            writer.Write(unknownStructure0.Count, 32u);
            unknownStructure0.ForEach(u => u.Write(writer));
        }
    }
}
